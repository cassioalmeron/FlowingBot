import React, { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import './styles.css';
import { api } from '../../../services/api';

const NewCollection: React.FC = () => {
  const [collectionName, setCollectionName] = useState('');
  const [files, setFiles] = useState<File[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  const onDrop = useCallback((acceptedFiles: File[]) => {
    setFiles(prevFiles => [...prevFiles, ...acceptedFiles]);
  }, []);

  const { getRootProps, getInputProps, isDragActive } = useDropzone({
    onDrop,
    multiple: true
  });

  const removeFile = (index: number) => {
    setFiles(prevFiles => prevFiles.filter((_, i) => i !== index));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    setError(null);
    setSuccess(null);
    try {
      await api.collections.create(collectionName, files);
      setSuccess('Collection created successfully!');
      setCollectionName('');
      setFiles([]);
    } catch {
      setError('Failed to create collection.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="collection-container">
      <form className="collection-form" onSubmit={handleSubmit}>
        <div className="collection-input-group">
          <label className="collection-label" htmlFor="collectionName">Collection Name</label>
          <input
            id="collectionName"
            className="collection-input"
            type="text"
            value={collectionName}
            onChange={(e) => setCollectionName(e.target.value)}
            placeholder="Enter collection name"
            required
          />
        </div>

        <div className="collection-input-group">
          <label className="collection-label">Files</label>
          <div {...getRootProps()} className={`collection-dropzone${isDragActive ? ' active' : ''}`}>
            <input {...getInputProps()} />
            <p className="collection-upload-message">
              {isDragActive
                ? 'Drop the files here...'
                : 'Drag and drop files here, or click to select files'}
            </p>
          </div>

          {files.length > 0 && (
            <ul className="collection-file-list">
              {files.map((file, index) => (
                <li key={index} className="collection-file-item">
                  <span>{file.name}</span>
                  <button onClick={() => removeFile(index)} type="button">Remove</button>
                </li>
              ))}
            </ul>
          )}
        </div>

        {error && <div style={{ color: 'red' }}>{error}</div>}
        {success && <div style={{ color: 'green' }}>{success}</div>}

        <button 
          className="collection-submit-button"
          type="submit"
          disabled={!collectionName || files.length === 0 || loading}
        >
          {loading ? 'Submitting...' : 'Create Collection'}
        </button>
      </form>
    </div>
  );
};

export default NewCollection;