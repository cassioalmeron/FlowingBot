import { useTheme } from '../../Utils/ThemeContext'
import { useState, useEffect } from 'react'
import { api, Configuration } from '../../services/api'
import { showToast } from '../../Utils/toast'

const Config = () => {
    const { isDarkMode, toggleDarkMode } = useTheme()
    const [embeddingModel, setEmbeddingModel] = useState('')
    const [openAIKey, setOpenAIKey] = useState('')
    const [source, setSource] = useState('OpenAI')
    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)

    useEffect(() => {
        loadConfigurations()
    }, [])

    const loadConfigurations = async () => {
        try {
            setLoading(true)
            const configurations = await api.configurations.getAll()
            
            // Map configurations to form fields
            configurations.forEach(config => {
                if (config.key === 'ModelName')
                    setEmbeddingModel(config.value)
                else if (config.key === 'OpenAIKey')
                    setOpenAIKey(config.value)
                else if (config.key === 'Source')
                    setSource(config.value)
            })
        } catch (error) {
            console.error('Error loading configurations:', error)
            showToast.error('Failed to load configurations')
        } finally {
            setLoading(false)
        }
    }

    const handleSave = async () => {
        try {
            setSaving(true)
            const configurations: Configuration[] = [
                { key: 'ModelName', value: embeddingModel },
                { key: 'OpenAIKey', value: openAIKey },
                { key: 'Source', value: source }
            ]
            
            await api.configurations.save(configurations)
            showToast.success('Configuration saved successfully')
        } catch (error) {
            console.error('Error saving configuration:', error)
            showToast.error('Failed to save configuration')
        } finally {
            setSaving(false)
        }
    }

    if (loading) {
        return (
            <div className="config-container">
                <h2>Configurations</h2>
                <div className="loading">Loading configurations...</div>
            </div>
        )
    }

    return (
        <div className="config-container">
            <h2>Configurations</h2>
            
            <div className="config-form">
                <div className="form-group">
                    <label htmlFor="embedding-model">Embedding Model</label>
                    <input
                        type="text"
                        id="embedding-model"
                        value={embeddingModel}
                        onChange={(e) => setEmbeddingModel(e.target.value)}
                        placeholder="Enter embedding model"
                        className="form-input"
                    />
                </div>

                <div className="form-group">
                    <label htmlFor="openai-key">Open AI Key</label>
                    <input
                        type="text"
                        id="openai-key"
                        value={openAIKey}
                        onChange={(e) => setOpenAIKey(e.target.value)}
                        placeholder="Enter your OpenAI API key"
                        className="form-input"
                    />
                </div>

                <div className="form-group">
                    <label>Source</label>
                    <div className="radio-group">
                        <label className="radio-option">
                            <input
                                type="radio"
                                name="source"
                                value="OpenAI"
                                checked={source === 'OpenAI'}
                                onChange={(e) => setSource(e.target.value)}
                            />
                            <span>OpenAI</span>
                        </label>
                        <label className="radio-option">
                            <input
                                type="radio"
                                name="source"
                                value="Ollama"
                                checked={source === 'Ollama'}
                                onChange={(e) => setSource(e.target.value)}
                            />
                            <span>Ollama</span>
                        </label>
                    </div>
                </div>

                <div className="form-group">
                    <label>Theme</label>
                    <button 
                        onClick={toggleDarkMode} 
                        className="theme-toggle"
                        aria-label={isDarkMode ? 'Switch to light mode' : 'Switch to dark mode'}
                    >
                        {isDarkMode ? '☀️' : '🌙'}
                    </button>
                </div>

                <div className="form-actions">
                    <button 
                        onClick={handleSave} 
                        className="save-button"
                        disabled={saving}
                    >
                        {saving ? 'Saving...' : 'Save'}
                    </button>
                </div>
            </div>
        </div>
    )
}

export default Config