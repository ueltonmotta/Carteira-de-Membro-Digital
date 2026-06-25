const CACHE_NAME = 'adhel-cache-v1';

self.addEventListener('install', (event) => {
    console.log('[Service Worker] Instalado');
    self.skipWaiting();
});

self.addEventListener('activate', (event) => {
    console.log('[Service Worker] Ativado');
    event.waitUntil(clients.claim());
});

self.addEventListener('fetch', (event) => {
    // Apenas responde à requisição normalmente, mas o navegador reconhece que o fetch existe
    event.respondWith(fetch(event.request));
});