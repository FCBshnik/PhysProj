<script lang="ts">
  import { localStorageService } from "$lib/services/LocalStorageService";
  import { onMount } from 'svelte';
  import gear from '$lib/components/icons/Gear.svelte'
	import Gear from "$lib/components/icons/Gear.svelte";

  const settingsKey = "settings:theme";

  class ThemeSettings {
    public isDark:boolean = true;
  }

  let settings:ThemeSettings = localStorageService.get<ThemeSettings>(settingsKey) || new ThemeSettings()
  setDarkMode(settings.isDark)

  function toggle() {
    settings.isDark = !settings.isDark;
    localStorageService.set(settingsKey, settings)
    setDarkMode(settings.isDark)
  }

  function setDarkMode(isDark:boolean) {
    document.documentElement.classList.toggle('dark', isDark)
  }
</script>

<button type="button" id="theme-toggle" class="" on:click={toggle}>
  <Gear></Gear>
</button>