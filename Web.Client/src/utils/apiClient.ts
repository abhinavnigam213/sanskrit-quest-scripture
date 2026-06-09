import axios from 'axios';

// Read credentials from Vite environment variables (prefixed with VITE_ to be exposed to the browser)
// These are set in the root .env file and loaded via the `envDir` setting in vite.config.ts
const CLIENT_ID = import.meta.env.VITE_CLIENT_ID as string;
const CLIENT_SECRET = import.meta.env.VITE_CLIENT_SECRET as string;

if (!CLIENT_ID || !CLIENT_SECRET) {
  console.error(
    '[Auth] Missing VITE_CLIENT_ID or VITE_CLIENT_SECRET in environment. ' +
    'Please set them in your root .env file.'
  );
}

let cachedToken: string | null = null;
let tokenExpiry: number | null = null; // timestamp in ms

/**
 * Fetches a new JWT token from the C# Auth controller.
 */
async function fetchNewToken(): Promise<string | null> {
  try {
    const response = await axios.post('/api/auth/token', {
      clientId: CLIENT_ID,
      clientSecret: CLIENT_SECRET,
    }, {
      // Prevent recursive interceptor execution on this auth request
      headers: {
        'X-Skip-Auth': 'true'
      }
    });

    if (response.status === 200 && response.data?.token) {
      cachedToken = response.data.token;
      tokenExpiry = new Date(response.data.expiresAt).getTime();
      console.log('[Auth] Successfully acquired new JWT Bearer token.');
      return cachedToken;
    }
  } catch (error) {
    console.error('[Auth] Failed to retrieve JWT Bearer token:', error);
  }
  return null;
}

/**
 * Custom Axios client configured for Sanskrit Quest APIs.
 */
const apiClient = axios.create({
  headers: {
    'Content-Type': 'application/json',
  },
});

// Request Interceptor: Automatically handles token acquisition and injection
apiClient.interceptors.request.use(
  async (config) => {
    // If the request explicitly requests to skip auth (e.g. token generation), proceed directly
    if (config.headers?.['X-Skip-Auth'] === 'true') {
      delete config.headers['X-Skip-Auth'];
      return config;
    }

    const now = Date.now();
    // Fetch a new token if there isn't one cached, or if it expires in less than 10 seconds
    if (!cachedToken || !tokenExpiry || tokenExpiry - now < 10000) {
      const token = await fetchNewToken();
      if (token) {
        cachedToken = token;
      }
    }

    if (cachedToken && config.headers) {
      config.headers['Authorization'] = `Bearer ${cachedToken}`;
    }

    return config;
  },
  (error) => {
    return Promise.reject(error);
  }
);

export default apiClient;
