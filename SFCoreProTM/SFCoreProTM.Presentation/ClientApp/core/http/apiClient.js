// ClientApp/core/http/apiClient.js
import axios from 'axios';
// Base Axios instance
const apiClient = axios.create({
  baseURL: '/', // Sesuaikan dengan base endpoint API kamu
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true, // jika kamu butuh kirim cookie atau token,
  timeout: 10000
});

// Optional: Interceptor untuk log, auth, dll
apiClient.interceptors.request.use(
  config => {
    // Tambahkan token jika ada
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  error => Promise.reject(error)
);

apiClient.interceptors.response.use(
  response => response.data,
  error => {
    console.error('API error:', error);
    return Promise.reject(error);
  }
);

export default apiClient;