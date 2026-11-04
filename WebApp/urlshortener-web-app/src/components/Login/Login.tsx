import { InteractionStatus } from '@azure/msal-browser'
import { useIsAuthenticated, useMsal } from '@azure/msal-react'
import { loginRequest } from '../../authConfig'
import Home from '../Home/Home'

function Login() {
  const { instance, inProgress } = useMsal()
  const isAuthenticated = useIsAuthenticated()

  // MsalProvider already processes the redirect response on startup,
  // so the button only needs to start the login.
  const onLogin = () => {
    instance.loginRedirect(loginRequest).catch((error) => console.error(error))
  }

  if (isAuthenticated) {
    return <Home />
  }

  return (
    <header className="App-header">
      <p>Welcome to Url Shortener!</p>
      <button
        className="login-button"
        onClick={onLogin}
        disabled={inProgress !== InteractionStatus.None}
      >
        Log in
      </button>
    </header>
  )
}

export default Login
