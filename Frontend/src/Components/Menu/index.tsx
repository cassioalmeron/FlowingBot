import React, { useState, useEffect } from 'react'
import { NavLink } from 'react-router-dom'
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
        console.log('Menu items response:', response)
        if (Array.isArray(response)) {
          setMenuItems(response)
        } else {
          console.warn('Response is not an array:', response)
          setMenuItems([])
        }
      } catch (err) {
        console.error('Error fetching menu items:', err)
        setError('Failed to load menu items')
        setMenuItems([])
      }
    }

    fetchMenuItems()
  }, [])

  if (error) return <div>Error: {error}</div>

  return (
    <nav className="main-navigation">
      <div className="nav-links-section">
        <NavLink to="/" className="nav-link" end>Home</NavLink>
        <NavLink to="/collection" className="nav-link">Collections</NavLink>
        <NavLink to="/config" className="nav-link">Config</NavLink>
        <NavLink to="/tests" className="nav-link">Tests</NavLink>
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
