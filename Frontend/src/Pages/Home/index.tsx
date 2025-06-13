import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import './styles.css';
import { api } from '../../services/api';
import { showToast } from '../../Utils/toast';

const Home: React.FC = () => {
  const [chatTopic, setChatTopic] = useState('');
  const [collections, setCollections] = useState<string[]>([]);
  const [selectedCollection, setSelectedCollection] = useState('');
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const loadCollections = async () => {
      try {
        const data = await api.collections.getAll();
        setCollections(data);
      } catch (error) {
        showToast.error('Error loading collections');
        console.error(error);
      } finally {
        setLoading(false);
      }
    };

    loadCollections();
  }, []);

  const handleStartChat = async () => {
    if (!chatTopic.trim()) {
      showToast.warning('Please enter a chat topic');
      return;
    }

    if (!selectedCollection) {
      showToast.warning('Please select a collection');
      return;
    }

    try {
      const data = await api.chat.create(chatTopic, selectedCollection);
      navigate(`/chat/${data.id}?topic=${encodeURIComponent(chatTopic)}`);
    } catch (error) {
      showToast.error('Error starting chat');
      console.error(error);
    }
  };

  return (
    <div className="home-container">
      <h1>Welcome to Flowing BOT</h1>
      <p>Your intelligent conversational assistant</p>
      <div className="chat-start-section">
        <select
          value={selectedCollection}
          onChange={(e) => setSelectedCollection(e.target.value)}
          className="collection-select"
          required
        >
          <option value="">Select a collection</option>
          {collections.map((collection, index) => (
            <option key={index} value={collection}>
              {collection}
            </option>
          ))}
        </select>
        <input 
          type="text" 
          placeholder="Optional: Describe your chat topic"
          value={chatTopic}
          onChange={(e) => setChatTopic(e.target.value)}
          className="chat-topic-input"
        />
        <button 
          onClick={handleStartChat} 
          className="start-chat-button"
          disabled={loading}
        >
          {loading ? 'Loading...' : 'Start a Chat'}
        </button>
      </div>
    </div>
  );
}

export default Home;