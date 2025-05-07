import React from 'react'
import ReactDOM from 'react-dom/client'
import Intro from './components/intro'

window.MountReactComponents = {
    renderIntro: (elementId) => {
        const container = document.getElementById(elementId)
        if (container) {
            ReactDOM.createRoot(container).render(
                <React.StrictMode>
                    <Intro/>
                </React.StrictMode>
            )
        }
    }
}