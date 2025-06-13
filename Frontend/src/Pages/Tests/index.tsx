import React from 'react';
import './styles.css';
import Streaming from './Streaming';
import Upload from './Upload';
import NewCollection from './NewCollection';

const Tests: React.FC = () => {
  return (
    <div className="tests-page">
      <NewCollection />
    </div>
  );
}

export default Tests;