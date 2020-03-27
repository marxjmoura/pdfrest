var formData = new FormData();

formData.append('file', pdfFile);
formData.append('conformanceLevel', 'PDF_A_2B');
formData.append('title', 'My Title');
formData.append('author', 'Author name');
formData.append('creationDate', new Date());
formData.append('customProperties[0].name', 'MyProperty');
formData.append('customProperties[0].value', 'My Property Value');

axios({
  method: 'post',
  url: '/pdfa',
  data: formData,
  config: {
    headers: {
      'Content-Type': 'multipart/form-data'
    }
  }
}).then(function (response) {
  // handle success
}).catch(function (response) {
  // handle error
});
