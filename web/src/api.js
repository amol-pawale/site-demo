// Empty base = same origin (local dev, via the Vite proxy).
// In production set VITE_API_URL to your HTTPS API address.
const base = (import.meta.env.VITE_API_URL || '').replace(/\/$/, '')

async function get(path) {
  const res = await fetch(`${base}${path}`)
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}`)
  return res.json()
}

export const getHello = () => get('/api/hello')
export const getProjects = () => get('/api/projects')
