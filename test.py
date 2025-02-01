import socket
import numpy as np
import time
from scipy.signal import find_peaks
from collections import deque
#from bitalino import BITalino
import csv
import os

# Configuration de BITalino
MAC_ADDRESS = "20:17:09:18:59:61"
SAMPLING_RATE = 100  # Fréquence d'échantillonnage (100 Hz recommandé pour PPG)
CHANNEL = [0]  # Canal du capteur PPG
CH = 5

# Configuration du socket UDP pour envoyer à Unity
UDP_IP = "127.0.0.1"  # Adresse locale (ou IP de l'ordinateur exécutant Unity)
UDP_PORT = 5005  # Port d'écoute côté Unity
sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

# Buffers pour stocker les RR-intervals des 60 dernières secondes et 10 dernières secondes
RR_BUFFER_1MIN = deque(maxlen=100)
RR_BUFFER_10S = deque(maxlen=15)

def calculate_hr_and_hrv(ppg_signal, sampling_rate):
    """Calcule la fréquence cardiaque (HR) et la variabilité (HRV) sur 10s et 1min"""
    # Détection des pics du signal PPG
    peaks, _ = find_peaks(ppg_signal, distance=sampling_rate/2)  # Distance min ~0.5s entre pics
    
    if len(peaks) < 2:
        return None, None, None, None  # Pas assez de données pour calculer HR et HRV
    
    rr_intervals = np.diff(peaks) / sampling_rate  # Intervalle RR en secondes
    RR_BUFFER_1MIN.extend(rr_intervals)  # Stockage dans le buffer 1 min
    RR_BUFFER_10S.extend(rr_intervals)  # Stockage dans le buffer 10 sec

    print(f"RR_BUFFER_1MIN: {len(RR_BUFFER_1MIN)}, RR_BUFFER_10S: {len(RR_BUFFER_10S)}")
    
    if len(RR_BUFFER_1MIN) < 2 or len(RR_BUFFER_10S) < 2:
        return None, None, None, None
    
    hr_1min = 60.0 / np.mean(RR_BUFFER_1MIN)  # BPM basé sur 1 min
    hrv_1min = np.std(RR_BUFFER_1MIN) * 1000  # HRV en ms basé sur 1 min
    
    hr_10s = 60.0 / np.mean(RR_BUFFER_10S)  # BPM basé sur 10 sec
    hrv_10s = np.std(RR_BUFFER_10S) * 1000  # HRV en ms basé sur 10 sec
    
    return hr_1min, hrv_1min, hr_10s, hrv_10s

def send_realtime_data():
    """Envoie les données en temps réel à Unity via un socket UDP"""
    
    try:
        print("Connexion à BITalino...")
        device = BITalino(MAC_ADDRESS)
        device.start(SAMPLING_RATE, CHANNEL)
        
        while True:
            raw_data = device.read(1000)  # Lire 1000 échantillons (10 sec environ)
            ppg_signal = raw_data[:, 5]  # Extraire le canal PPG
            
            hr_1min, hrv_1min, hr_10s, hrv_10s = calculate_hr_and_hrv(ppg_signal, SAMPLING_RATE)
                
            if hr_1min is not None and hrv_1min is not None and hr_10s is not None and hrv_10s is not None:
                data_to_send = f"{hr_1min:.2f},{hrv_1min:.2f},{hr_10s:.2f},{hrv_10s:.2f}"
                sock.sendto(data_to_send.encode(), (UDP_IP, UDP_PORT))
                print(f"HR(1min): {hr_1min:.2f} BPM, HRV(1min): {hrv_1min:.2f} ms, HR(10s): {hr_10s:.2f} BPM, HRV(10s): {hrv_10s:.2f} ms - Envoyé à Unity")
            
            time.sleep(1)  # Pause avant la prochaine lecture

    except KeyboardInterrupt:
        print("Arrêt du script.")
        device.stop()
        device.close()
        sock.close()

def send_sample_data():
    """Envoie des données d'exemple à Unity pour tester l'interface"""
    data = os.path.join(os.path.dirname(__file__), "simulation.csv")
    with open(data, "r") as file:
        reader = csv.reader(file)
        next(reader)  # Ignorer l'en-tête
        for row in reader:
            data_to_send = ",".join(row)
            sock.sendto(data_to_send.encode(), (UDP_IP, UDP_PORT))
            print(f"Envoyé à Unity: {data_to_send}")
            time.sleep(10)
    # try:
    #     while True:
    #         data_to_send = "75.00,10.00,80.00,15.00"  # HR(1min), HRV(1min), HR(10s), HRV(10s)
    #         sock.sendto(data_to_send.encode(), (UDP_IP, UDP_PORT))
    #         print(f"Envoyé à Unity: {data_to_send}")
    #         time.sleep(1)
    # except KeyboardInterrupt:
    #     print("Arrêt du script.")
    #     sock.close()
    
if __name__ == "__main__":
    #send_realtime_data()
    send_sample_data()