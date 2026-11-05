import type { ShortUrl } from '../../types'
import './ListUrls.css'
import {useState} from "react";

interface ListUrlsProps {
  urls: ShortUrl[]
  continuationToken: string | null
  loading: boolean
  onLoadMore: () => void
  onDelete: (id: string) => Promise<void>
}

function ListUrls({ urls, continuationToken, loading, onLoadMore, onDelete }: ListUrlsProps) {

    const [deletingId, setDeletingId] = useState<string | null>(null)

    const handleDelete = async (id: string) => {
        if (!window.confirm(`Delete ${id}? The short link will stop working.`)) return
        setDeletingId(id)
        await onDelete(id)
        setDeletingId(null)
    }
    
  return (
    <div className="url-list">
      {urls.map((url) => (
          <div key={url.id} className="url-item">
          <span className="url-links">
            <a href={url.shortUrl} className="short-url">{url.id}</a>
              {' → '}
              <a href={url.longUrl} className="long-url">{url.longUrl}</a>
          </span>
              <button
                  className="delete-button"
                  onClick={() => handleDelete(url.id)}
                  disabled={deletingId === url.id}
              >
                  {deletingId === url.id ? 'Deleting…' : 'Delete'}
              </button>
          </div>
      ))}
      {continuationToken && (
        <button onClick={onLoadMore} disabled={loading}>
          {loading ? 'Loading…' : 'Load more'}
        </button>
      )}
    </div>
  )
}

export default ListUrls
