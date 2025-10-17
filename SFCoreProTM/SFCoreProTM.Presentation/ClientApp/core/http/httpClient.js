import apiClient from './apiClient'

export async function get(url, config = {}) {
  return apiClient.get(url, config)
}

export async function post(url, data = {}, config = {}) {
  return apiClient.post(url, data, config)
}

export async function put(url, data = {}, config = {}) {
  return apiClient.put(url, data, config)
}

export async function del(url, config = {}) {
  return apiClient.delete(url, config)
}

export async function uploadSingleFile(url, file, config = {}) {
  const formData = new FormData()
  formData.append('file', file)
  return apiClient.post(url, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    ...config,
  })
}

export async function uploadMultipleFiles(url, files = [], config = {}) {
  const formData = new FormData()
  files.forEach((file, i) => formData.append(`files[${i}]`, file))
  return apiClient.post(url, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    ...config,
  })
}

export async function uploadFileWithData(url, fileFieldName, file, otherData = {}, config = {}) {
  const formData = new FormData()
  formData.append(fileFieldName, file)
  Object.entries(otherData).forEach(([key, val]) => formData.append(key, val))
  return apiClient.post(url, formData, {
    headers: { 'Content-Type': 'multipart/form-data' },
    ...config,
  })
}