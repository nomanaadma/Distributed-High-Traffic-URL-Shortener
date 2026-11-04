import { useState, type FormEvent } from 'react'
import './UrlForm.css'

interface UrlFormProps {
  onSubmit: (longUrl: string) => Promise<void>
}

function UrlForm({ onSubmit }: UrlFormProps) {
  const [longUrl, setLongUrl] = useState('')
  const [submitting, setSubmitting] = useState(false)

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    const value = longUrl.trim()
    if (!value) return

    setSubmitting(true)
    try {
      await onSubmit(value)
      // Only clear the input once the URL was saved, so nothing is lost on failure.
      setLongUrl('')
    } catch {
      // The parent shows the error; keep what the user typed.
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <form className="url-form" onSubmit={handleSubmit}>
      <input
        type="url"
        required
        placeholder="Enter a long URL"
        value={longUrl}
        onChange={(e) => setLongUrl(e.target.value)}
        className="url-input"
        aria-label="Long URL"
      />
      <button type="submit" className="shorten-button" disabled={submitting}>
        {submitting ? 'Shortening…' : 'Shorten'}
      </button>
    </form>
  )
}

export default UrlForm
