import { BrowserRouter as Router, Routes, Route } from 'react-router-dom'
import { ToastContainer } from 'react-toastify'
import 'react-toastify/dist/ReactToastify.css'
import Menu from './Components/Menu'
import './App.css'
import Chat from './Pages/Chat'
import Home from './Pages/Home'
import Tests from './Pages/Tests'
import Collection from './Pages/Collection'
import { ThemeProvider } from './Utils/ThemeContext'
import Config from './Pages/Config'

function App() {
  return (
    <ThemeProvider>
      <Router>
        <div className="container">
          <Menu />
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/chat/:chatId" element={<Chat />} />
            <Route path="/tests" element={<Tests />} />
            <Route path="/collection" element={<Collection />} />
            <Route path="/config" element={<Config />} />
          </Routes>
          <ToastContainer
            position="top-right"
            autoClose={3000}
            hideProgressBar={false}
            newestOnTop
            closeOnClick
            rtl={false}
            pauseOnFocusLoss
            draggable
            pauseOnHover
            theme="colored"
          />
        </div>
      </Router>
    </ThemeProvider>
  )
}

export default App