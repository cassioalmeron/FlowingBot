import '@testing-library/jest-dom';

// Polyfill for TextEncoder/TextDecoder
import { TextEncoder, TextDecoder } from 'util';
if (typeof global.TextEncoder === 'undefined') {
  global.TextEncoder = TextEncoder as typeof global.TextEncoder;
}
if (typeof global.TextDecoder === 'undefined') {
  global.TextDecoder = TextDecoder as typeof global.TextDecoder;
}

// Mock window.matchMedia
Object.defineProperty(window, 'matchMedia', {
  writable: true,
  value: jest.fn().mockImplementation(query => ({
    matches: false,
    media: query,
    onchange: null,
    addListener: jest.fn(),
    removeListener: jest.fn(),
    addEventListener: jest.fn(),
    removeEventListener: jest.fn(),
    dispatchEvent: jest.fn(),
  })),
});

// Mock import.meta.env for Vite
const globalAny = globalThis as Record<string, unknown>;
if (!('import' in globalAny)) {
  globalAny.import = {};
}
const importObj = globalAny.import as Record<string, unknown>;
if (!('meta' in importObj)) {
  importObj.meta = {};
}
const metaObj = importObj.meta as Record<string, unknown>;
metaObj.env = {
  VITE_API_BASE_URL: 'http://localhost:3000', // or whatever default you want
}; 