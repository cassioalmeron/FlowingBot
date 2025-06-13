import { useState } from 'react'
import { ReplaceStreamingEmbeddings } from '../../../Utils/Util'

const Streaming = () => {
    const [streamedText, setStreamedText] = useState<string>('');
    const [isStreaming, setIsStreaming] = useState<boolean>(false);
    
    const handleStreamRequest = async () => {
        setIsStreaming(true);
        setStreamedText('');

        try {
            const response = await fetch('https://localhost:7024/api/Streaming', {
                method: 'POST'
            });

            if (response.body) {
                const reader = response.body.getReader();
                const decoder = new TextDecoder("utf-8");

                while (true) {
                    const { done, value } = await reader.read();
                    if (done) break;

                    let chunk = decoder.decode(value);
                    chunk = ReplaceStreamingEmbeddings(chunk);

                    setStreamedText(prev => `${prev}${chunk}`);
                }
            }
        } catch (error) {
            console.error('Streaming error:', error);
        } finally {
            setIsStreaming(false);
        }
    }

    return (
        <div className="tests-page">
            <button 
                onClick={handleStreamRequest} 
                disabled={isStreaming}>
                {isStreaming ? 'Streaming...' : 'Start Streaming'}
            </button>

            {streamedText && (
                <div className="streamed-content">
                    <h2>Streamed Content:</h2>
                    <pre><code>{streamedText}</code></pre>
                </div>
            )}
        </div>
    );
}

export default Streaming