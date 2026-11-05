import axios from 'axios'
import type {ShortUrl, UrlPage} from '../types'

const apiEndpoint = import.meta.env.VITE_API_ENDPOINT
const PAGE_SIZE = 5

export async function getUrls(token: string, continuation: string | null): Promise<UrlPage> {
  const response = await axios.get<UrlPage>(`${apiEndpoint}/api/urls`, {
    headers: { Authorization: `Bearer ${token}` },
    params: { continuation, pageSize: PAGE_SIZE },
  })
  return response.data
}

export async function createUrl(token: string, longUrl: string): Promise<ShortUrl> {
    const response = await axios.post<ShortUrl>(
        `${apiEndpoint}/api/urls`,
        { LongUrl: longUrl },
        { headers: { Authorization: `Bearer ${token}` } },
    )
    return response.data
}

export async function deleteUrl(token: string, id: string): Promise<void> {
    await axios.delete(`${apiEndpoint}/api/urls/${encodeURIComponent(id)}`, {
        headers: { Authorization: `Bearer ${token}` },
    })
}