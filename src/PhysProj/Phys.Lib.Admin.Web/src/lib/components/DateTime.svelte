<script lang="ts">
    export let dateTime:Date|undefined = undefined
</script>

{#if dateTime != undefined}
  {dateTime.toISOString().split('T')[0]}<br/>
  {dateTime.toISOString().split('T')[1]}
{/if}

