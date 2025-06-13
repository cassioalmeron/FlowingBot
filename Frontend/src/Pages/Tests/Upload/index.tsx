import React, { useState, useCallback } from 'react';
import { useDropzone } from 'react-dropzone';
import styled from 'styled-components';

// import { Container } from './styles';

const Container = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 2rem;
  border: 2px dashed #ccc;
  border-radius: 8px;
  background-color: #fafafa;
  cursor: pointer;
  transition: border-color 0.3s ease;

  &:hover {
    border-color: #2196f3;
  }
`;

const UploadMessage = styled.p`
  margin: 1rem 0;
  color: #666;
`;

const FileList = styled.ul`
  list-style: none;
  padding: 0;
  margin: 1rem 0;
  width: 100%;
  max-width: 400px;
`;

const FileItem = styled.li`
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.5rem;
  margin: 0.5rem 0;
  background-color: #fff;
  border-radius: 4px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
`;

const Upload: React.FC = () => {
  const [files, setFiles] = useState<File[]>([]);

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

  return (
    <Container {...getRootProps()}>
      <input {...getInputProps()} />
      {isDragActive ? (
        <UploadMessage>Drop the files here...</UploadMessage>
      ) : (
        <UploadMessage>Drag and drop files here, or click to select files</UploadMessage>
      )}
      
      {files.length > 0 && (
        <FileList>
          {files.map((file, index) => (
            <FileItem key={index}>
              <span>{file.name}</span>
              <button onClick={() => removeFile(index)}>Remove</button>
            </FileItem>
          ))}
        </FileList>
      )}
    </Container>
  );
};

export default Upload;