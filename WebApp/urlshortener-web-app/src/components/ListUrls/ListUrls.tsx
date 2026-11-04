import type { ShortUrl } from '../../types'
import './ListUrls.css'

interface ListUrlsProps {
  urls: ShortUrl[]
  continuationToken: string | null
  loading: boolean
  onLoadMore: () => void
}

function ListUrls({ urls, continuationToken, loading, onLoadMore }: ListUrlsProps) {
  return (
    <div className="url-list">
      {urls.map((url) => (
        <div key={url.shortUrl} className="url-item">
          <a href={url.shortUrl} className="short-url">
            {url.id}
          </a>
          {' → '}
          <a href={url.longUrl} className="long-url">
            {url.longUrl}
          </a>
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
