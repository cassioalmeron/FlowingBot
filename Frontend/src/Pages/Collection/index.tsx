import React, { useEffect, useState } from 'react'
import { api } from '../../services/api';
import { showToast } from '../../Utils/toast';
import './styles.css';

const Collection: React.FC = () => {
    const [collections, setCollections] = useState<string[]>([]);
    const [selectedCollection, setSelectedCollection] = useState('');
    const [query, setQuery] = useState('');
    const [queryResults, setQueryResults] = useState<string[]>([]);
    const [loading, setLoading] = useState(true);
    const [queryLoading, setQueryLoading] = useState(false);

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

    const handleQuery = async () => {
        if (!selectedCollection) {
          showToast.warning('Please select a collection');
          return;
        }
    
        if (!query.trim()) {
          showToast.warning('Please enter a query');
          return;
        }
    
        setQueryLoading(true);
        try {
          const results = await api.collections.query(query, selectedCollection);
          setQueryResults(results);
        } catch (error) {
          showToast.error('Error querying collection');
          console.error(error);
        } finally {
          setQueryLoading(false);
        }
      };
  
    return (
    <div className="collection-container">
        <h1>Collection Query</h1>
        <div className="collection-form">
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
              placeholder="Enter your query"
              value={query}
              onChange={(e) => setQuery(e.target.value)}
              className="query-input"
            />

            <button 
              onClick={handleQuery} 
              className="query-button"
              disabled={loading || queryLoading}
            >
              {queryLoading ? 'Querying...' : 'Query'}
            </button>

            {queryResults.length > 0 && (
              <div className="query-results">
                <h2>Results</h2>
                <ul className="results-list">
                  {queryResults.map((result, index) => (
                    <li key={index} className="result-item">
                      {result}
                    </li>
                  ))}
                </ul>
              </div>
            )}
        </div>
    </div>
  )
}

export default Collection