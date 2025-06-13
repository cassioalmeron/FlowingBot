import axios from 'axios';

const BASE_URL = 'https://localhost:7024/api';

// Types
export interface Chat {
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

// API functions
export const api = {
  // Chat endpoints
  chat: {
    // Get all chats
    getAll: async (): Promise<Chat[]> => {
      const response = await axios.get<Chat[]>(`${BASE_URL}/Chat`);
      return response.data;
    },

    // Get messages for a specific chat
    getMessages: async (chatId: string): Promise<Message[]> => {
      const response = await axios.get<Message[]>(`${BASE_URL}/Chat/${chatId}/Messages`);
      return response.data;
    },

    // Create a new chat
    create: async (userPrompt: string): Promise<{ id: number }> => {
      const response = await axios.post<{ id: number }>(
        `${BASE_URL}/Chat?userPrompt=${encodeURIComponent(userPrompt)}`
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
  }
}; 