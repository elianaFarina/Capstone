import 'bootstrap/dist/css/bootstrap.min.css';
import React from 'react';
import ReactDOM from 'react-dom/client';
import Intro from './components/intro';

// Definisci l'oggetto globale in modo esplicito
window.MountReactComponents = {
    renderIntro: (elementId) => {
        const container = document.getElementById(elementId);
        if (container) {
            ReactDOM.createRoot(container).render(
                <React.StrictMode>
                    <Intro />
                </React.StrictMode>
            );
        }
    }
};

// Esporta anche come modulo
export const MountReactComponents = window.MountReactComponents;