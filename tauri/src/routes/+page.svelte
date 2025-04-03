<script lang="ts">
  import { type UnlistenFn } from "@tauri-apps/api/event";
  import { getCurrentWebview } from "@tauri-apps/api/webview";
  import { onDestroy, onMount } from "svelte";

  let dndState: "waiting" | "dragging" | "dropped" = $state("waiting");
  let droppedPaths: string[] = $state([]);

  const unlistens: UnlistenFn[] = [];
  onMount(async () => {
    let unlisten: UnlistenFn;
    unlisten = await getCurrentWebview().onDragDropEvent((event) => {
      if (event.payload.type === "over") {
        dndState = "dragging";
        console.log("User hovering", event.payload.position);
      } else if (event.payload.type === "drop") {
        dndState = "dropped";
        console.log("User dropped", event.payload.paths);
        const paths = event.payload.paths;
        if (paths && paths.length > 0) {
          droppedPaths = paths;
        }
      } else {
        dndState = "waiting";
        console.log("File drop cancelled");
      }
    });
  });

  onDestroy(() => {
    for (const unlisten of unlistens) {
      unlisten();
    }
  });

  const clearDroppedPathss = () => {
    droppedPaths = [];
    dndState = "waiting";
  };
</script>

<main class="container">
  <h1>Drag and Drop Test</h1>

  {#if dndState == "waiting"}
    <div>Drag files here.</div>
  {:else if dndState == "dragging"}
    <div>Drop them here!</div>
  {:else if dndState == "dropped"}
    <div>Files successfully dropped. Thank you! 🤗</div>
  {/if}

  {#if droppedPaths.length > 0}
    <h2>Dropped File Path</h2>
    <ul>
      {#each droppedPaths as path}
        <li>{path}</li>
      {/each}
    </ul>
    <button onclick={() => clearDroppedPathss()}>Clear</button>
  {/if}
</main>

<style>
</style>
