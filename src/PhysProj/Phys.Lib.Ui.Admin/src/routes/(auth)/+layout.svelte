<script lang="ts">
  import * as api from '$lib/services/ApiService';
  import { goto } from '$app/navigation';

  if (!api.service.isAuthorized())
      goto("/login");

  function logout() {
      api.service.clearToken();
      goto("/login");
  }
</script>

<nav>
  <ul>
    <li><strong>Admin</strong></li>
  </ul>
  <ul>
    <li>
      <a href="/home">Home</a>
    </li>
    <li>
      <button on:click={logout}>Logout</button>
    </li>
  </ul>
</nav>

<slot />