import type { IPublicClientApplication } from '@azure/msal-browser'
import { MsalProvider } from '@azure/msal-react'
import Login from './components/Login/Login'
import './App.css'

interface AppProps {
  instance: IPublicClientApplication
}

function App({ instance }: AppProps) {
  return (
    <MsalProvider instance={instance}>
      <div className="App">
        <Login />
      </div>
    </MsalProvider>
  )
}

export default App
