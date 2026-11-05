import { useCallback, useEffect, useState } from 'react'
import { InteractionRequiredAuthError } from '@azure/msal-browser'
import { useMsal } from '@azure/msal-react'
import { apiRequest } from '../../authConfig'
import { createUrl, getUrls, deleteUrl } from '../../api/urls'
import type { ShortUrl, UrlPage } from '../../types'
import ListUrls from '../ListUrls/ListUrls'
import LogoutButton from '../LogoutButton/LogoutButton'
import UrlForm from '../UrlForm/UrlForm'
import './Home.css'

const LOAD_ERROR = 'Could not load your URLs. Check your connection and try again.'

function Home() {
  const { instance, accounts } = useMsal()
  const account = accounts[0]

  const [urls, setUrls] = useState<ShortUrl[]>([])
  const [continuationToken, setContinuationToken] = useState<string | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const getToken = useCallback(async () => {
    try {
      const response = await instance.acquireTokenSilent({ ...apiRequest, account })
      return response.accessToken
    } catch (err) {
      // Silent renewal can fail when the session expired or consent is needed.
      // Fall back to an interactive login instead of failing every API call.
      if (err instanceof InteractionRequiredAuthError) {
        await instance.acquireTokenRedirect({ ...apiRequest, account })
      }
      throw err
    }
  }, [instance, account])

    const handleDelete = async (id: string) => {
        setError(null)
        try {
            const token = await getToken()
            await deleteUrl(token, id)
            setUrls((prev) => prev.filter((u) => u.id !== id))
        } catch (err) {
            console.error(err)
            setError('Could not delete that URL. Try again.')
        }
    }

  const applyPage = (page: UrlPage, reset: boolean) => {
    setUrls((prev) => (reset ? page.urls : [...prev, ...page.urls]))
    setContinuationToken(page.continuationToken)
  }

  const fetchPage = useCallback(
    async (continuation: string | null) => getUrls(await getToken(), continuation),
    [getToken],
  )

  // reset = true loads the first page and replaces the list;
  // otherwise the next page is appended.
  const loadUrls = async (continuation: string | null, reset: boolean) => {
    setLoading(true)
    setError(null)
    try {
      applyPage(await fetchPage(continuation), reset)
    } catch (err) {
      console.error(err)
      setError(LOAD_ERROR)
    } finally {
      setLoading(false)
    }
  }

  // Initial load, once on mount. State is only set after the request finishes,
  // and a response that arrives after unmount (or StrictMode's dev re-run) is ignored.
  useEffect(() => {
    let ignore = false
    fetchPage(null)
      .then((page) => {
        if (!ignore) applyPage(page, true)
      })
      .catch((err) => {
        console.error(err)
        if (!ignore) setError(LOAD_ERROR)
      })
      .finally(() => {
        if (!ignore) setLoading(false)
      })
    return () => {
      ignore = true
    }
  }, [fetchPage])

  const handleLoadMore = () => {
    void loadUrls(continuationToken, false)
  }

  const handleSubmit = async (longUrl: string) => {
    setError(null)
    try {
      const token = await getToken()
      const created = await createUrl(token, longUrl)
      setUrls((prev) => [created, ...prev])
    } catch (err) {
      console.error(err)
      setError('Could not shorten that URL. Try again.')
      throw err
    }
  }

  const handleLogout = () => {
    instance.logoutRedirect({ account }).catch((err) => console.error(err))
  }

  return (
    <div className="container">
      <h1>URL Shortener</h1>
      <div className="header">
        <LogoutButton onLogout={handleLogout} />
      </div>
      <UrlForm onSubmit={handleSubmit} />
      {error && (
        <p className="error-message" role="alert">
          {error}
        </p>
      )}
      <ListUrls
        urls={urls}
        continuationToken={continuationToken}
        loading={loading}
        onLoadMore={handleLoadMore}
        onDelete={handleDelete}
      />
    </div>
  )
}

export default Home
