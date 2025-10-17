import httpClient from './httpClient'

export const authService = {
  async signUp(payload) {
    const response = await httpClient.post('/auth/signup', payload)
    return response.data
  },

  async signIn(payload) {
    const response = await httpClient.post('/auth/login', payload)
    return response.data
  }
}

export default authService
