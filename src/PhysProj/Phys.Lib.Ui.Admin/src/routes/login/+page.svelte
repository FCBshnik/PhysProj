<script lang="ts">
    import * as api from '$lib/services/ApiService';
    import { goto } from '$app/navigation';

    let username = '';
    let password = '';

    goHome();

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
        }).catch(err => console.info(err));
	}
</script>

<article class="grid">
    <div>
      <hgroup>
        <h1>Sign in</h1>
      </hgroup>
      <form>
        <input
          bind:value={username}
          type="text"
          name="login"
          placeholder="Login"
          aria-label="Login"
        />
        <input
        bind:value={password}
          type="password"
          name="password"
          placeholder="Password"
          aria-label="Password"
        />
        <button type="submit" on:click={login}>Login</button>
      </form>
    </div>
    <div></div>
  </article>