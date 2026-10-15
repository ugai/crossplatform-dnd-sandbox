const dropArea = document.getElementById('drop');
const filenamesElem = document.getElementById('filenames');

const hasFiles = (e) => e.dataTransfer.types.includes('Files');

// Prevent Chromium from navigating to a file dropped outside the drop area
document.addEventListener('dragover', (e) => e.preventDefault());
document.addEventListener('drop', (e) => e.preventDefault());

dropArea.addEventListener('dragover', (e) => {
  e.preventDefault();
  if (hasFiles(e)) dropArea.classList.add('drag-over');
});

dropArea.addEventListener('dragleave', () => {
  dropArea.classList.remove('drag-over');
});

dropArea.addEventListener('drop', (e) => {
  e.preventDefault();
  dropArea.classList.remove('drag-over');

  // getFilePath returns "" for items that are not on the file system
  const paths = [...e.dataTransfer.files]
    .map((f) => electron.getFilePath(f))
    .filter((p) => p !== '');
  if (paths.length === 0) return;

  console.log('File(s) you dragged here: ', paths);
  filenamesElem.innerText = paths.join('\n');
});
