module.exports = {
  presets: [
    ['@babel/preset-env', { targets: { node: 'current' } }],
    ['@babel/preset-react', { runtime: 'automatic' }],
    '@babel/preset-typescript',
  ],
  plugins: [
    ['babel-plugin-transform-import-meta', {
      replace: {
        env: {
          VITE_API_BASE_URL: 'http://localhost:3000',
        },
      },
    }],
  ],
}; 