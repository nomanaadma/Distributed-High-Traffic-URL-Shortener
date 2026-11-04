import './LogoutButton.css'

interface LogoutButtonProps {
  onLogout: () => void
}

function LogoutButton({ onLogout }: LogoutButtonProps) {
  return (
    <button onClick={onLogout} className="logout">
      Logout
    </button>
  )
}

export default LogoutButton
