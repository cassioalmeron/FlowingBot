import React, { useState, useEffect } from 'react'
import { NavLink } from 'react-router-dom'
import { useTheme } from '../../Utils/ThemeContext'
import './styles.css'
import { api } from '../../services/api'

interface MenuItem {
  id: number
  title: string
}

const Menu: React.FC = () => {
  const [menuItems, setMenuItems] = useState<MenuItem[]>([])
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const fetchMenuItems = async () => {
      try {
        const response = await api.chat.getAll()
        setMenuItems(response)
      } catch (err) {
        console.error('Error fetching menu items:', err)
        setError('Failed to load menu items')
      }
    }

    fetchMenuItems()
  }, [])

  if (error) return <div>Error: {error}</div>

  return (
    <nav className="main-navigation">
      <div className="nav-header">
        <div className="nav-links">
          <NavLink to="/" className="nav-link" end>Home</NavLink>
        </div>
      </div>
      <hr className="nav-divider" />
      <div className="dynamic-menu-items">
        {menuItems.map((item) => (
          <NavLink 
            key={item.id} 
            to={`/chat/${item.id}`} 
            className={({ isActive }) => 
              `menu-item ${isActive ? 'menu-item-active' : ''}`
            }
          >
            {item.title}
          </NavLink>
        ))}
      </div>
    </nav>
  )
}

export default Menu
