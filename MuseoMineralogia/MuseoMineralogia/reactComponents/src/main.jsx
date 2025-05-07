import React from 'react'
import ReactDOM from 'react-dom/client'
import Counter from './components/testComponent'

window.MountReactComponents = {
    renderCounter: (elementId, props = {}) => {
        const container = document.getElementById(elementId)
        if (container) {
            ReactDOM.createRoot(container).render(
                <React.StrictMode>
                    <Counter {...props} />
                </React.StrictMode>
            )
        }
    }
}