window.audioInterop = {
    playAudioFromBase64: function (base64Data, format) {
        return new Promise((resolve, reject) => {
            try {
                const audioData = atob(base64Data);
                const bytes = new Uint8Array(audioData.length);
                for (let i = 0; i < audioData.length; i++) {
                    bytes[i] = audioData.charCodeAt(i);
                }
                const mimeType = format === 'mp3' ? 'audio/mpeg' : 'audio/wav';
                const blob = new Blob([bytes], { type: mimeType });
                const url = URL.createObjectURL(blob);
                const audio = new Audio(url);
                audio.onended = () => {
                    URL.revokeObjectURL(url);
                    resolve(true);
                };
                audio.onerror = (e) => {
                    URL.revokeObjectURL(url);
                    reject('Audio playback error: ' + e.message);
                };
                audio.play();
            } catch (err) {
                reject('Error playing audio: ' + err);
            }
        });
    },
    stopAudio: function () {
        const audios = document.querySelectorAll('audio');
        audios.forEach(a => { a.pause(); a.currentTime = 0; });
    }
};
