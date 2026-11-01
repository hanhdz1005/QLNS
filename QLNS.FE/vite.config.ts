import { fileURLToPath, URL } from 'node:url'
import net from 'node:net'
import { defineConfig, type ProxyOptions } from 'vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'
import tailwindcss from '@tailwindcss/vite'

const BACKEND_CANDIDATES = [
  { url: 'https://localhost:7065', port: 7065 },
  { url: 'http://localhost:5000', port: 5000 },
  { url: 'http://localhost:5034', port: 5034 },
]

function isPortListening(port: number, host: string): Promise<boolean> {
  return new Promise((resolve) => {
    const socket = new net.Socket()
    const done = (ok: boolean) => {
      socket.destroy()
      resolve(ok)
    }
    socket.setTimeout(250)
    socket.once('connect', () => done(true))
    socket.once('timeout', () => done(false))
    socket.once('error', () => done(false))
    socket.connect(port, host)
  })
}

let cachedTarget = 'http://localhost:5000'
let lastCheckedAt = 0

async function resolveBackendTarget(): Promise<string> {
  const now = Date.now()
  if (now - lastCheckedAt < 3000) {
    return cachedTarget
  }
  for (const candidate of BACKEND_CANDIDATES) {
    const okIpv4 = await isPortListening(candidate.port, '127.0.0.1')
    const okIpv6 = okIpv4 || (await isPortListening(candidate.port, '::1'))
    if (okIpv6) {
      cachedTarget = candidate.url
      lastCheckedAt = now
      return cachedTarget
    }
  }
  return cachedTarget
}

function createSmartProxy(): ProxyOptions {
  let proxyInstance: { options: { target?: unknown } } | null = null
  return {
    target: cachedTarget,
    changeOrigin: true,
    secure: false,
    configure: (proxy) => {
      proxyInstance = proxy as unknown as { options: { target?: unknown } }
    },
    bypass: async (_req, _res, options) => {
      const target = await resolveBackendTarget()
      options.target = target
      if (proxyInstance?.options) {
        proxyInstance.options.target = target
      }
      return undefined
    },
  }
}

// https://vite.dev/config/
export default defineConfig(async () => {
  await resolveBackendTarget()

  return {
    plugins: [vue(), tailwindcss(), vueDevTools()],
    resolve: {
      alias: {
        '@': fileURLToPath(new URL('./src', import.meta.url)),
      },
    },
    server: {
      port: 5173,
      proxy: {
        '/api': createSmartProxy(),
        '/uploads': createSmartProxy(),
      },
    },
  }
})
