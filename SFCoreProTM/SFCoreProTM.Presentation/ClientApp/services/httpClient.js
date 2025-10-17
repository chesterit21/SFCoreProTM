import axios from 'axios'

const instance = axios.create({
  baseURL: '/api',
  headers: {
    'Content-Type': 'application/json'
  },
  withCredentials: true
})

instance.interceptors.response.use(
  (response) => response,
  (error) => {
    const fallbackMessage = 'Something went wrong. Please try again.'
    if (error.response) {
      const { data } = error.response
      const message =
        data?.detail ||
        data?.title ||
        data?.message ||
        (Array.isArray(data?.errors)
          ? data.errors.join(', ')
          : fallbackMessage)
      return Promise.reject({
        status: error.response.status,
        message,
        raw: error.response.data
      })
    }

    if (error.request) {
      return Promise.reject({
        status: 0,
        message: 'Unable to reach the server. Check your connection.',
        raw: error
      })
    }

    return Promise.reject({
      status: 0,
      message: fallbackMessage,
      raw: error
    })
  }
)

export default instance
