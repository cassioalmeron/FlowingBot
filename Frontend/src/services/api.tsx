import axios from 'axios';

const BASE_URL = 'https://localhost:7024/api';

// Types
export interface MenuMessage {
  id: number;
  title: string;
}

export interface Message {
  id: number;
  chatId: number;
  role: string;
  text: string;
  timestamp: string;
  chat: object | null;
}

export interface CollectionResponse {
  id: number;
  name: string;
  files: string[];
}

// API functions
export const api = {
  // Chat endpoints
  chat: {
    // Get all chats
    getAll: async (): Promise<MenuMessage[]> => {
      const response = await axios.get<MenuMessage[]>(`${BASE_URL}/Chat`);
      return response.data;
    },

    // Get messages for a specific chat
    getMessages: async (chatId: string): Promise<Message[]> => {
      const response = await axios.get<Message[]>(`${BASE_URL}/Chat/${chatId}/Messages`);
      return response.data;
    },

    // Create a new chat
    create: async (userPrompt: string, selectedCollection: string): Promise<{ id: number }> => {
      const response = await axios.post<{ id: number }>(
        `${BASE_URL}/Chat?userPrompt=${encodeURIComponent(userPrompt)}&collection=${encodeURIComponent(selectedCollection)}`
      );
      return response.data;
    },

    // Send a message to a chat
    sendMessage: async (chatId: string, userPrompt: string): Promise<Response> => {
      return fetch(
        `${BASE_URL}/Chat/${chatId}/Messages?userPrompt=${encodeURIComponent(userPrompt)}`,
        { method: 'POST' }
      );
    }
  },

  // Streaming endpoint
  streaming: {
    start: async (): Promise<Response> => {
      return fetch(`${BASE_URL}/Streaming`, { method: 'POST' });
    }
  },

  // Collections endpoint
  collections: {
    create: async (name: string, files: File[]): Promise<CollectionResponse> => {
      const formData = new FormData();
      formData.append('name', name);
      files.forEach((file) => {
        formData.append('files', file);
      });
      const response = await axios.post<CollectionResponse>(`${BASE_URL}/Collection`, formData, {
        headers: {
          'Content-Type': 'multipart/form-data',
        },
      });
      return response.data;
    },

    getAll: async (): Promise<string[]> => {
      const response = await axios.get<string[]>(`${BASE_URL}/Collection`);
      return response.data;
    },

    query: async (query: string, collection: string): Promise<string[]> => {
      const response = await axios.get<string[]>(`${BASE_URL}/Collection/Query?query=${query}&collection=${collection}`);
      return response.data;
    },
  },
}; 