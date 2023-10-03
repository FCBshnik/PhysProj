<script lang="ts">
    import DateTime from '$lib/components/DateTime.svelte';
import * as api from '$lib/services/ApiService';
    
    let migrators:api.MigratorModel[] = [];
    let migrations:api.MigrationModel[] = [];

    let selectedMigrator: api.MigratorModel;
    let selectedSource:string;
    let selectedDestination:string;

    api.service.listMigrators().then(r => migrators = r);
    refresh();

    async function refresh() {
      migrations = await api.service.listMigrations();
    }

    async function start() {
      api.service.startMigration({ migrator: selectedMigrator.name, source: selectedSource, destination: selectedDestination });
    }
</script>

<article>
  <section class="p-2">Migrations</section>
  <section class="flex flex-row items-center p-4 gap-4">
    <div class="w-auto">
      Migrator
    </div>
    <div class="w-auto">
      <select bind:value={selectedMigrator}>
        {#each migrators as m}
          <option value={m}>{m.name}</option>
        {/each}
      </select>
		</div>
    {#if selectedMigrator?.sources && selectedMigrator?.destinations}
    <div class="w-auto">
      <select bind:value={selectedSource}>
        {#each selectedMigrator.sources as s}
          <option value={s}>{s}</option>
        {/each}
      </select>
		</div>
    <div class="w-auto">
      <select bind:value={selectedDestination}>
        {#each selectedMigrator.destinations as s}
          <option value={s}>{s}</option>
        {/each}
      </select>
		</div>
    {/if}
    <button class="w-auto" on:click={start} disabled={selectedMigrator == undefined}>Start</button>
    <button class="w-auto" on:click={refresh}>Refresh</button>
  </section>
  <section class="flex flex-row items-center p-4 gap-4">
    <table class="table-auto w-full">
      <thead class="">
        <tr class="border-b-2 border-b-gray-700">
          <th>Id</th>
          <th>Name</th>
          <th>Source</th>
          <th>Destination</th>
          <th>Status</th>
          <th>Start</th>
          <th>End</th>
          <th>Count</th>
          <th>Result</th>
        </tr>
      </thead>
      <tbody>
        {#each migrations as migration}
        <tr class="border-t-2 border-t-gray-700">
          <td>{migration.id}</td>
          <td>{migration.migrator}</td>
          <td>{migration.source}</td>
          <td>{migration.destination}</td>
          <td>{migration.status}</td>
          <td><DateTime dateTime={migration.createdAt}/></td>
          <td><DateTime dateTime={migration.completedAt}/></td>
          <td>{migration.migratedCount}</td>
          <td>{migration.result}</td>
        </tr>
        {#if migration.error}
        <tr>
          <td colspan=9>{migration.error}</td>
        </tr>
        {/if}
        {/each}
      </tbody>
    </table>
  </section>
</article>