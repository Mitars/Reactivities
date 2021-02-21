import React, { useRef } from 'react';
import Cropper from 'react-cropper';
import 'cropperjs/dist/cropper.css';

const PhotoWidgetCropper = ({
  setImage,
  imagePreview,
}: {
  setImage: (file: Blob) => void;
  imagePreview: string;
}) => {
  const cropperRef = useRef<HTMLImageElement>(null);

  const cropImage = () => {
    const imageElement: any = cropperRef?.current;
    const imageCropper: any = imageElement?.cropper;
    imageCropper
      ?.getCroppedCanvas()
      .toBlob((blob: any) => setImage(blob), 'image/jpeg');
  };

  return (
    <Cropper
      src={imagePreview}
      style={{ height: 200, width: '100%' }}
      // Cropper.js options
      initialAspectRatio={1 / 1}
      aspectRatio={1 / 1}
      guides={false}
      viewMode={1}
      dragMode="move"
      scalable={true}
      cropBoxMovable={true}
      cropBoxResizable={true}
      crop={cropImage}
      preview=".img-preview"
      ref={cropperRef}
    />
  );
};

export default PhotoWidgetCropper;
