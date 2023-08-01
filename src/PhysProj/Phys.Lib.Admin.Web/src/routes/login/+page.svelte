<script lang="ts">
    import * as api from '$lib/services/ApiService';
    import { goto } from '$app/navigation';

    let username = '';
    let password = '';

    function goHome(){
      if (api.service.isAuthorized())
        goto("/home");
    }

    async function login() {
        await api.service.login(new api.LoginModel({ username: username, password: password }))
        .then(res => {
            console.info('login token: ', res.token);
            api.service.setToken(res.token);
            goHome();
        }).catch(() => {});
    }
</script>

<section>
  <form>
    <header>Login</header>
    <input bind:value={username} type="text" placeholder="Name">
    <input bind:value={password} type="password" placeholder="Password">
    <button on:click={login}>Sign In</button>
  </form>
</section>