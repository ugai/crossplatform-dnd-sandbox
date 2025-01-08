document.addEventListener('drop', (e) => {
  e.preventDefault();
  e.stopPropagation();
});

document.getElementById('drop').addEventListener('drop', (e) => {
  e.preventDefault();
  e.stopPropagation();

  let s = "";
  for (const f of e.dataTransfer.files) {
    p = electron.getFilePath(f)
    s += p + "\n"
    console.log('File(s) you dragged here: ', p)
  }

  const filenamesElem = document.getElementById('filenames')
  filenamesElem.innerText = s
});

document.addEventListener('dragover', (e) => {
  e.preventDefault();
  e.stopPropagation();
});
