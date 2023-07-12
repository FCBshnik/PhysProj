<script lang="ts">
	import * as api from '$lib/services/ApiService';
	import { goto } from '$app/navigation';

	if (!api.service.isAuthorized()) 
		goto('/login');

	function logout() {
		api.service.clearToken();
		goto('/login');
	}
</script>

<nav>
	<a href="/home">Admin</a>
	<ul>
		<a href="/home">Home</a>
		<a href="/authors">Authors</a>
    <a href="/works">Works</a>
		<button on:click={logout}>Logout</button>
	</ul>
</nav>

<slot />
