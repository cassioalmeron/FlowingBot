import React, { useState, useEffect, useRef, useCallback } from 'react'
import { useParams, useLocation } from 'react-router-dom'
import ReactMarkdown from 'react-markdown'
import './styles.css'
import { ReplaceStreamingEmbeddings } from '../../Utils/Util'
import { api, Message } from '../../api'
import Spinner from '../../Components/Spinner'
import { showToast } from '../../Utils/toast'

const Chat: React.FC = () => {
  const { chatId = '1' } = useParams()
  const location = useLocation();
  const [messages, setMessages] = useState<Message[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [isStreaming, setIsStreaming] = useState<boolean>(false);
  const [newMessage, setNewMessage] = useState('');
  const messagesEndRef = useRef<HTMLDivElement>(null);

  const scrollToBottom = () => {
    messagesEndRef.current?.scrollIntoView({ behavior: 'smooth' })
  }
  
  useEffect(() => {
    const params = new URLSearchParams(location.search);
    const queryTopic = params.get('topic');
    if (queryTopic){
      setLoading(false);
      return;
    }

    const fetchMessages = async () => {
      try {
        const data = await api.chat.getMessages(chatId);
        setMessages(data);

        // Scroll to bottom after messages are loaded
        setTimeout(scrollToBottom, 100)
      } catch (err) {
        console.error('Error fetching messages:', err)
        showToast.error('Failed to load messages')
        setError('Failed to load messages')
      }
      finally {
        setLoading(false);
      }
    }

    fetchMessages()
  }, [chatId, location.search])

  const sendMessage = useCallback(async (userPrompt: string) => {
    if (!userPrompt.trim() || isStreaming) return

    try {
      setIsStreaming(true);
      // Create user message object
      const userMessage: Message = {
        id: Date.now(), // Temporary unique ID
        chatId: parseInt(chatId),
        role: 'User',
        text: userPrompt,
        timestamp: new Date().toISOString(),
        chat: null
      }

      // Create a temporary assistant message for streaming
      const tempAssistantMessage: Message = {
        id: Date.now() + 1,
        chatId: parseInt(chatId),
        role: 'Assistant',
        text: '',
        timestamp: new Date().toISOString(),
        chat: null
      }

      // Add temporary assistant message
      setMessages(prevMessages => [...prevMessages, userMessage, tempAssistantMessage])

      // Send message and stream response
      const response = await api.chat.sendMessage(chatId, userPrompt);

      if (response.body) {
        const reader = response.body.getReader();
        const decoder = new TextDecoder('utf-8');

        // Update the last message (the temporary assistant message)
        let accumulatedText = '';
        while (true) {
          const { done, value } = await reader.read();
          if (done) break;

          let chunk = decoder.decode(value);
          console.log(chunk);
          chunk = ReplaceStreamingEmbeddings(chunk);
          accumulatedText += chunk;

          // Update messages with the incrementally growing text
          setMessages(prevMessages => {
            const updatedMessages = [...prevMessages];
            const lastMessageIndex = updatedMessages.length - 1;
            updatedMessages[lastMessageIndex] = {
              ...updatedMessages[lastMessageIndex],
              text: accumulatedText
            }
            return updatedMessages;
          })
        }
      }
    } catch (err) {
      console.error('Error sending message:', err)
      showToast.error('Failed to send message')
      setError('Failed to send message')
    } finally {
      setIsStreaming(false);
    }
  }, [chatId, setMessages, setError, isStreaming]);

  const handleSendMessage = async () => {
    sendMessage(newMessage)
    setNewMessage('')
  }

  const hasRun = useRef(false);

  useEffect(() => {
    if (hasRun.current) return; 
    hasRun.current = true;
    const params = new URLSearchParams(location.search);
    const queryTopic = params.get('topic');
    if (queryTopic && !isStreaming){
      sendMessage(queryTopic);
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])
      
  if (loading)
    return <div>Loading messages...</div>

  if (error)
    return <div>Error: {error}</div>

  return (
    <div className='content'>
      <div className="messages">
        {messages.map((message) => (
          <div 
            key={message.id} 
            className={`message ${message.role === 'User' ? 'user-message' : 'assistant-message'}`}
          >
            <div className='message-header'>
              <span className='timestamp'>
                {new Date(message.timestamp).toLocaleString()}
              </span>
            </div>
            <div className='message-text'>
              <ReactMarkdown>{message.text}</ReactMarkdown>
            </div>
          </div>
        ))}
        <div ref={messagesEndRef} />
      </div>
      <div className='chat-input'>
        <textarea
          value={newMessage}
          onChange={(e) => setNewMessage(e.target.value)}
          placeholder='Type your message...'
          disabled={isStreaming}
        />
        {isStreaming ? (
          <div className="spinner-container">
            <Spinner />
          </div>
        ) : (
          <button onClick={handleSendMessage}>
            Send
          </button>
        )}
      </div>
    </div>
  )
}

export default Chat