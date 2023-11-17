<script lang="ts">
    import { notificationsService } from "$lib/services/NotificationsService"

    var message:string|undefined = undefined
    var timeout:number = 0

    notificationsService.on('message', m => {
        if (timeout)
            clearTimeout(timeout)
        message = m
        timeout = setTimeout(close, 10000)
    })

    function close(){
      message = undefined;
    }
</script>

{#if message}
    <button on:click={close} class="absolute top-5 right-5 max-w-md w-full rounded-md p-4 dark:bg-gray-300 dark:text-gray-600 dark:bg-opacity-90 bg-gray-500 text-gray-200 bg-opacity-90">
        {message}
    </button>
{/if}