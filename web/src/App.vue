<script setup>
import { onMounted, ref } from 'vue'
import { getHello, getProjects } from './api'

const hello = ref(null)
const projects = ref([])
const error = ref('')
const loading = ref(true)

async function load() {
  loading.value = true
  error.value = ''
  try {
    const [h, p] = await Promise.all([getHello(), getProjects()])
    hello.value = h
    projects.value = p
  } catch (e) {
    error.value = `Could not reach the API (${e.message}).`
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <main class="page">
    <header class="hero">
      <h1>Amol's Demo Site</h1>
      <p>A Vue frontend on Cloudflare Pages, talking to a .NET 10 API on Oracle Cloud.</p>
    </header>

    <section class="card">
      <div class="card-head">
        <h2>API status</h2>
        <button @click="load" :disabled="loading">{{ loading ? 'Loading…' : 'Refresh' }}</button>
      </div>

      <p v-if="error" class="error">{{ error }}</p>

      <dl v-else-if="hello" class="status">
        <dt>Message</dt>
        <dd>{{ hello.message }}</dd>
        <dt>Server time (UTC)</dt>
        <dd>{{ new Date(hello.serverTimeUtc).toLocaleString() }}</dd>
        <dt>Uptime</dt>
        <dd>{{ hello.uptimeSeconds }} seconds</dd>
        <dt>Runtime</dt>
        <dd>{{ hello.runtime }}</dd>
      </dl>
    </section>

    <section v-if="projects.length" class="card">
      <h2>Projects (from the API)</h2>
      <ul class="projects">
        <li v-for="p in projects" :key="p.id">
          <strong>{{ p.name }}</strong>
          <span>{{ p.description }}</span>
          <span class="tags">
            <em v-for="t in p.tags" :key="t">{{ t }}</em>
          </span>
        </li>
      </ul>
    </section>
  </main>
</template>
