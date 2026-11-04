import type { Configuration, PopupRequest } from '@azure/msal-browser'

const appUri = import.meta.env.PROD
? window.location.origin // This will use the static web app URL in production
: 'http://localhost:3000';

export const msalConfig: Configuration = {
  auth: {
    clientId: import.meta.env.VITE_CLIENT_ID,
    authority: import.meta.env.VITE_AUTHORITY,
    redirectUri: appUri,
    postLogoutRedirectUri: appUri,
  },
}

export const loginRequest: PopupRequest = {
  scopes: ['User.Read'],
}

// Each scope is its own array entry (openid and profile are added by MSAL automatically).
export const apiRequest = {
  scopes: [`api://${import.meta.env.VITE_CLIENT_ID}/Urls.Read`],
}
