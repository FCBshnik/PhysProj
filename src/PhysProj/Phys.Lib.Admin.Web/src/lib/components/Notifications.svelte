<script lang="ts">
    import { notificationsService } from "$lib/services/NotificationsService"

    var message:string|undefined = undefined
    var timeout:number = 0;

    notificationsService.on('message', m => {
        if (timeout)
            clearTimeout(timeout);
        message = m
        timeout = setTimeout(() => { message = undefined }, 5000);
    })
</script>

{#if message}
    <div class="notification">
        {message}
    </div>
{/if}