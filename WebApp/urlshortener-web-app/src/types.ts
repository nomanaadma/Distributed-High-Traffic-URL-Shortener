export interface ShortUrl {
  id: string
  shortUrl: string
  longUrl: string
}

export interface UrlPage {
  urls: ShortUrl[]
  continuationToken: string | null
}
