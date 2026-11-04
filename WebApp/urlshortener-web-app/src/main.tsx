import { createRoot } from 'react-dom/client'
import { PublicClientApplication } from '@azure/msal-browser'
import { msalConfig } from './authConfig'
import './index.css'
import App from './App'

// Created once, outside React, so StrictMode re-renders don't create a second instance.
// MsalProvider calls initialize() and handleRedirectPromise() on startup.
const msalInstance = new PublicClientApplication(msalConfig)

createRoot(document.getElementById('root')!).render(
    <App instance={msalInstance} />
)
