import { render } from '@testing-library/react';
import App from './App';

// Mock the components that might cause issues in tests
jest.mock('./Components/Menu', () => ({
  __esModule: true,
  default: () => <div data-testid="menu">Menu</div>,
}));

jest.mock('./Components/Spinner', () => ({
  __esModule: true,
  default: () => <div data-testid="spinner">Spinner</div>,
}));

// Mock react-markdown to avoid ESM issues
jest.mock('react-markdown', () => ({
  __esModule: true,
  default: (props: React.PropsWithChildren<object>) => <div data-testid="react-markdown">{props.children}</div>,
}));

// Mock services/api to avoid import.meta.env issues
jest.mock('./services/api', () => ({
  __esModule: true,
  api: {},
  Message: {},
}));

describe('App', () => {
  test('renders without crashing', () => {
    render(<App />);
    // Basic test to ensure the app renders
    expect(document.body).toBeInTheDocument();
  });
}); 