HAZIR BETON OPERASYON YÖNETİM SİSTEMİ — Proje Planı

1. Projenin temel amacı
Bu sistemin amacı, hazır beton operasyonunda yer alan tüm temel süreçleri tek bir merkezden yönetilebilir hale getirmektir.
Sistem şu ihtiyaçlara cevap vermelidir:
* müşteri ve şantiye bazlı beton taleplerini toplamak
* talepleri operasyon tarafından sisteme girmek
* yönetici onayı ve randevu sürecini yönetmek
* uygun aracı manuel olarak atamak
* araç, personel ve operasyon ilişkisini görünür kılmak
* gün sonu gerçekleşen teslim miktarını kaydetmek
* müşteriye otomatik bilgilendirme göndermek
* günlük, haftalık, aylık ve yıllık maliyetleri analiz etmek
* gün sonu raporlarını ve ertesi gün planını üretmek
* dış sistemlerden veri alarak merkezi bir yönetim ekranı oluşturmak
* ileriki aşamalarda araç canlı takibini ve müşteri taraflı teslimat görünürlüğünü desteklemek
Bu sistem bir ERP’nin tamamı değildir.Ama sıradan bir takip paneli de değildir.En doğru tanımıyla bu proje:
Hazır beton üretim, planlama, sevk, teslim, maliyet ve izleme süreçlerini yöneten merkezi bir operasyon yönetim sistemidir.

2. Sistemin ana yapısı
Sistem ana olarak aşağıdaki modüllerden oluşacaktır:
* Dashboard
* Program
* Filo
* Müşteriler
* Personel
* Maliyet
* Ayarlar
Bunlara ek olarak sistemin destek yapıları şunlardır:
* SMS bildirim sistemi
* Gün sonu raporlama sistemi
* Dış sistem entegrasyon yapısı
* İleri faz GPS / araç takip sistemi
* Çoklu platform erişim yapısı

3. Sistemin genel iş mantığı
Sistemin ana akışı şu mantıkla çalışır:
1. Operasyoncu bir müşteri adına beton talebi oluşturur.
2. Talep müşteri ve şantiye ile ilişkilendirilir.
3. Talep edilen tarih/saat sisteme girilir.
4. Talep başlangıçta “Onay Bekleyen” olur.
5. Yönetici talebi inceler.
6. Gerekirse müşterinin istediği saat değiştirilir ve uygun randevu verilir.
7. Talep onaylanır.
8. Onaylanan randevu bilgisi müşteriye SMS ile iletilir.
9. Yönetici uygun aracı manuel olarak atar.
10. Araç ile ilişkili esas ve yedek personel sistemde görünür.
11. Operasyon Dashboard ve takvim görünümünden izlenir.
12. Gün sonunda sahada gerçekleşen gerçek beton miktarı manuel girilir.
13. Talep “Teslim Edilen” durumuna çekilir.
14. Gerçek teslim miktarına göre müşteriye teslim SMS’i gider.
15. Kayıt gün sonu raporuna ve maliyet analizine dahil olur.
Bu omurga proje boyunca sabit kabul edilir.

4. Kullanıcı yapısı ve rol sistemi
Sistem rol ve yetki bazlı çalışacaktır.Ancak burada çok önemli bir sabit kural vardır:
Değiştirilemez ana kural
Kullanıcı oluşturma, kullanıcı düzenleme, rol atama ve yetki verme işlemleri sadece Ana Yönetici tarafından yapılabilir.
Bu yetki hiçbir şekilde başka kullanıcıya devredilemez.Hiçbir kullanıcıya “kullanıcı yönetimi” veya “yetki verme” hakkı tanımlanamaz.

4.1 Roller
Sistemde nihai olarak 5 temel kullanıcı rolü bulunacaktır:
* Ana Yönetici
* Yan Yönetici
* Operasyoncu
* Muhasebe / Finans Kullanıcısı
* İzleyici / Santralci

4.2 Rol tanımları
Ana Yönetici
Sistemdeki en yüksek yetkili kullanıcıdır.
Yapabilecekleri:
* tüm modülleri görüntüleme
* tüm modüllerde işlem yapma
* kullanıcı oluşturma
* kullanıcı düzenleme
* rol atama
* yetki verme / değiştirme
* rapor alma
* maliyet görme ve yönetme
* operasyonu yönetme
* sistem ayarlarını değiştirme
Yan Yönetici
Operasyonel yönetim yapabilen kullanıcıdır.
Yapabilecekleri:
* talepleri görüntüleme
* talepleri onaylama
* randevu düzenleme
* araç atama
* rapor alma
* operasyonu izleme
Yapamayacakları:
* kullanıcı oluşturma
* kullanıcı yönetme
* rol atama
* yetki verme
Operasyoncu
Talep oluşturma ve gün sonu gerçekleşme girişi yapan kullanıcıdır.
Yapabilecekleri:
* beton talebi oluşturma
* talep detaylarını görüntüleme
* gün sonu gerçek teslim miktarını girme
* operasyonel kayıtları takip etme
Yapamayacakları:
* talep onaylama
* randevu değiştirme
* kullanıcı yönetimi
* yetki verme
Muhasebe / Finans Kullanıcısı
Maliyet ve finansal analiz odaklı kullanıcıdır.
Yapabilecekleri:
* maliyet verilerini görüntüleme
* maliyet girişi yapma
* kârlılık analizlerini görme
* rapor alma
* yakıt / gider / maliyet verilerini inceleme
Yapamayacakları:
* talep onaylama
* araç atama
* kullanıcı yönetimi
* yetki verme
İzleyici / Santralci
Tamamen görüntüleme odaklı kullanıcıdır.
Yapabilecekleri:
* ekranları görüntüleme
* canlı durumu izleme
Yapamayacakları:
* hiçbir kayıt oluşturma
* hiçbir kayıt değiştirme
* hiçbir onay işlemi
* kullanıcı yönetimi
* maliyet müdahalesi

4.3 Yetki sistemi
Rol yapısına ek olarak sistemde modül bazlı ve işlem bazlı yetkilendirme olacaktır.
Örnek yetkiler:
* Dashboard görüntüleme
* Program görüntüleme
* Talep oluşturma
* Talep onaylama
* Randevu değiştirme
* Gün sonu kapatma
* Araç görüntüleme
* Araç düzenleme
* Maliyet görüntüleme
* Maliyet girişi
* Rapor alma
* PDF export
* Excel export
Ancak tekrar netleştirelim:
Kullanıcı yönetimi, rol atama ve yetki verme işlemleri yetkilendirilebilir bir işlem değildir; sadece Ana Yöneticiye aittir.

5. Ayarlar modülü
Sistemde ayrı bir Ayarlar modülü bulunacaktır.
Bu modül yalnızca Ana Yönetici tarafından erişilebilir olacaktır.
Bu modülde şunlar bulunacaktır:
* kullanıcı listesi
* yeni kullanıcı oluşturma
* kullanıcı pasife alma
* rol atama
* yetki düzenleme
* sistemsel konfigürasyonlar
* gerekirse bildirim ayarları
Bu modül diğer rollerde ya hiç görünmeyecek ya da erişime kapalı olacaktır.En temiz yaklaşım: sadece Ana Yönetici menüsünde görünmesi.

6. Dashboard
Dashboard, sistemin ana ekranıdır.Bu ekran bir modüller topluluğu değildir; ayrı bir ana sayfadır.
Dashboard’un rolü
* sistemin genel özetini göstermek
* kritik uyarıları görünür yapmak
* kullanıcıyı ilgili detay sayfalara yönlendirmek
Dashboard işlem ekranı değildir
Burada veri oluşturma veya ana iş akışı yürütme yapılmaz.Ana işlemler ilgili modüllerde yapılır.
Dashboard’da bulunacak alanlar
* günlük özet kartları
* onay bekleyen talepler
* bugünkü planlanan işler
* teslim edilen işler
* bakım uyarıları
* gün sonu kapatılmamış işler
* ertesi gün planı
* operasyon takvimi
Operasyon takvimi
Bu, Dashboard içinde yer alacak ayrı bir görünüm alanıdır.
Takvim:
* Program modülünün yerine geçmez
* talep oluşturma ekranı değildir
* zaman bazlı izleme ekranıdır
Burada görülebilecek bilgiler:
* saat
* müşteri
* şantiye
* teslim şekli
* malzeme türü
* miktar
* talep durumu
* atanmış araç
* ilgili operasyon durumu
Bir kayda tıklandığında kullanıcı talep detay ekranına gidebilir.

7. Program modülü
Program modülü sistemin en kritik iş akışı modülüdür.Beton taleplerinin oluşturulduğu, yönetildiği ve yaşam döngüsünün takip edildiği yerdir.

7.1 Beton talep formu
Beton talep formunda aşağıdaki bilgiler bulunacaktır:
* müşteri
* şantiye
* talep eden kişi
* firma yetkilisi telefon
* şantiye yetkilisi telefon
* malzeme / beton türü
* miktar
* fiyat
* irsaliye tipi
* teslim şekli
* talep edilen tarih / saat
Buradaki takvim alanı, yalnızca tarih ve saat seçimi için kullanılacaktır.Takvim burada form bileşenidir, ayrı bir planlama modülü değildir.

7.2 Müşteri ve şantiye ilişkisi
Bir müşteri birden fazla şantiyeye sahip olabilir.Bu nedenle talep oluşturma sırasında:
* önce müşteri seçilir
* sonra o müşteriye bağlı şantiyeler listelenir
Bu ilişki tüm sistem boyunca korunmalıdır.

7.3 Talep durumları
Sistemde talepler aşağıdaki durumlardan birinde olacaktır:
* Onay Bekleyen
* Onaylanan
* Teslim Edilen
* İptal Edilen
Durum akışı
* Talep oluşturulunca: Onay Bekleyen
* Yönetici onaylayınca: Onaylanan
* Gün sonu gerçek teslim girilip iş kapatılınca: Teslim Edilen
* Uygun görülmezse veya iptal edilirse: İptal Edilen

7.4 Talep edilen saat ve onaylanan randevu saati
Bu iki bilgi ayrı tutulmalıdır.
Talep edilen saat
Müşterinin ilk talebidir.
Onaylanan randevu saati
Yöneticinin nihai verdiği saattir.
Örneğin:
* müşteri 09:00 ister
* yönetici 10:00 verir
Bu yüzden sistemde tek saat alanı yeterli değildir.

7.5 Talep edilen miktar ve teslim edilen miktar
Bu iki bilgi de ayrı tutulmalıdır.
Talep edilen miktar
Müşterinin istediği miktardır.
Teslim edilen miktar
Gün sonunda sahada gerçekte dökülen miktardır.
Örnek:
* talep edilen: 100 m³
* teslim edilen: 98 m³
Sistem bu ayrımı korumalıdır.

7.6 Yönetici onayı ve randevu süreci
Talep oluşturulduktan sonra yönetici:
* talebi inceler
* gerekiyorsa saati değiştirir
* randevuyu onaylar
Onay sonrası müşteri talep kişisine SMS gider.
Örnek:“100 m³ C25 beton için saat 10:00’da randevunuz oluşturulmuştur.”

7.7 Araç atama
Araç atama kesin olarak manuel yapılacaktır.Sistem otomatik araç atamayacaktır.
Yönetici:
* uygun araçları görür
* bakım ve durum bilgisini kontrol eder
* kendisi araç seçer
Bu işlem Program modülündeki talep detay ekranında yapılmalıdır.

7.8 Gün sonu işlemi
Gün sonunda ilgili kullanıcı (örneğin operasyoncu / Yusuf):
* sahada gerçekten kaç m³ döküldüğünü girer
* talebi teslim edilmiş olarak kapatır
Bu manuel kapanıştır.Otomatik teslim mantığı yoktur.
Ardından müşteriye teslim SMS’i gider.
Örnek:“98 m³ C25 betonunuz teslim edilmiştir.”

8. Filo modülü
Filo modülü araç ve ekipman yönetimi için kullanılır.
Yönetilecek araç türleri
* beton mikseri
* pompa
* kepçe
* şantiye aracı
* servis aracı
Her araç için tutulacak temel bilgiler
* plaka
* araç tipi
* durum
* bakım bilgisi
* bağlı personel
Araç durumları
* Aktif
* Bakımda
* Pasif
* Sistem dışı
Not: geçmiş kayıtların bozulmaması için araçların tamamen silinmesi yerine pasif / sistem dışı mantığı daha sağlıklıdır.

8.1 Bakım yönetimi
Her araç için:
* son bakım tarihi
* bir sonraki bakım bilgisi
* bakım uyarı durumu
takip edilir.
Bakımı yaklaşan araçlar Dashboard’da görünür.
Bu araçlar operasyon için riskli kabul edilir ve talep atamasında yöneticiyi uyarır.
Bakım için SMS gönderilmeyecektir.Sadece sistem içi uyarı olacaktır.

8.2 Araç-personel ilişkisi
Her araç için:
* 1 esas personel
* 1 veya daha fazla yedek personel
tanımlanabilir.
Bu çok önemlidir çünkü:
* esas personel izne çıkabilir
* başka biri geçici görev alabilir
Talebe araç atandığında, araç üzerinden bağlı personel bilgisi görünmelidir.

9. Personel modülü
Personel modülü operasyonel personeli yönetmek için vardır.Tam bir insan kaynakları modülü değildir.
Tutulacak temel bilgiler
* ad soyad
* telefon
* görev tipi
* bağlı araç
* aktif / pasif durumu
Personel türleri örnek
* mikser şoförü
* pompa operatörü
* saha personeli
* servis şoförü
Temel mantık:Talep → Araç → Personel
Yani personel talebe doğrudan değil, araç üzerinden ilişkilenecektir.

10. Müşteriler modülü
Müşteri yönetimi modülü, cari / firma verisinin tutulduğu alandır.Ana veri kaynağı olarak Netsis düşünülmektedir.
Temel bilgiler
* firma adı
* cari kod
* telefon
* adres
* notlar
* bağlı şantiyeler
Bir müşteri birden fazla şantiyeye sahip olabilir.
Şantiye mantığı
* müşteri ayrı varlık
* şantiye ayrı varlık
* bir müşteri → birden fazla şantiye
Bu ayrım sistemin tüm operasyon mantığında korunacaktır.

11. Maliyet modülü
Maliyet modülü sistemin finansal analiz katmanıdır.Muhasebe programının tamamı değildir, ama operasyonel maliyet ve kârlılık üretmek için yeterli olmalıdır.

11.1 Maliyet verisi nasıl tutulacak?
Maliyetler kalem bazlı ve günlük giriş olarak tutulacaktır.
Yani:
* çimento
* mazot
* elektrik
* su
* kum
* personel gideri
* sanayi / bakım gideri
* diğer giderler
ayrı ayrı kaydedilecektir.
Toplam maliyet, sonradan bu kalemlerden hesaplanacaktır.
Çok kritik kural
Veri baştan kalem bazlı tutulmalıdır.Sadece toplam gider tutulmayacaktır.

11.2 Günlük giriş mantığı
Her maliyet girişi için örnek yapı:
* tarih
* maliyet kalemi
* miktar
* birim
* birim fiyat
* toplam tutar
* açıklama
Örnek:
* 08/04 → çimento → 20 ton
* 07/04 → mazot → 20000 litre
Fiyatlar değişken olabileceği için günlük bazda girilmesi doğrudur.

11.3 Hesaplama mantığı
Sistem, maliyet verisi ile operasyon / üretim verisini birleştirecektir.
Temel hesaplar:
* Toplam gider
* Toplam üretilen / teslim edilen beton
* Birim maliyet
* Toplam satış
* Kâr / zarar
Formül mantığı:
* Toplam gider = tüm maliyet girişleri toplamı
* Toplam üretim = teslim edilen toplam m³
* Birim maliyet = gider / m³
* Kâr / zarar = satış – gider

11.4 Analiz seviyeleri
Sistem zaman bazlı analiz yapabilmelidir:
* günlük
* haftalık
* aylık
* yıllık
Ayrıca:
* en yüksek gider kalemi
* gider dağılımı
* kâr / zarar eğilimi
gibi sonuçlar üretebilmelidir.
Grafikler
İlerleyen aşamalarda aşağıdaki görseller planlanmalıdır:
* en çok harcama yapılan kalem
* maliyet dağılımı
* dönemsel maliyet artışı
* kâr / zarar görünümü

12. SMS sistemi
SMS sistemi iki ana olay için çalışacaktır:
12.1 Onay / randevu SMS’i
Talep onaylandığında, beton talep kişisine gider.
İçerik:
* onaylanan randevu saati
* miktar
* beton türü
12.2 Teslim SMS’i
Gün sonunda gerçek teslim edilen miktar girildiğinde gider.
İçerik:
* gerçek teslim miktarı
* beton türü
İleri faz SMS / bildirimleri
GPS geldiğinde aşağıdaki olay bazlı bildirimler de düşünülebilir:
* araç çıktı
* araç yaklaştı
* araç belli mesafeye geldi
* teslimat tamamlandı

13. Raporlama
Sistemde raporlama ayrı bir değer üretmelidir.
13.1 Gün sonu raporu
Günlük olarak şunları içerebilir:
* bugün kaç iş yapıldı
* toplam kaç m³ beton teslim edildi
* hangi müşteriye ne kadar teslim edildi
* planlanan / gerçekleşen farkları
* gün sonu kapanmamış işler
13.2 Ertesi gün planı
* yarının onaylı işleri
* saat bazlı plan
* müşteri ve şantiye dağılımı
13.3 Dışa aktarma
Raporlar şu formatlarda alınabilmelidir:
* PDF
* Excel

14. Dış sistem entegrasyonu
Sistem dış sistemlerden veri alabilecek şekilde tasarlanacaktır.
14.1 Netsis
Ana kullanım amacı:
* müşteri / cari verisi almak
* gerekirse irsaliye / ticari veri okumak
Muhtemel erişim yöntemleri:
* NetOpenX
* API
* export
14.2 Olympos
Ana kullanım amacı:
* sevk kayıtları
* araç
* plaka
* şoför
* beton türü
* m³
* irsaliye bilgileri
Muhtemel erişim yöntemleri:
* API varsa API
* veritabanı erişimi
* rapor export / import
Entegrasyon yaklaşımı
İdeal hedef otomatik veri çekmektir.Eğer bu mümkün olmazsa, standart rapor export/import geçici çözüm olabilir.
Burada önemli ayrım şudur:
* manuel tek tek veri girişi başka şeydir
* dış sistemden toplu rapor alıp sisteme aktarmak başka şeydir

15. Dashboard + entegrasyon ilişkisi
Dashboard, dış sistemlerden ve kendi sisteminizden gelen verileri birleştiren görünüm katmanı olacaktır.
Örnek:
* Netsis’ten müşteri verisi gelir
* Olympos’tan sevk ve araç verisi gelir
* sizin sistemdeki talepler ve maliyetlerle birleşir
* Dashboard’da anlamlı operasyon resmi oluşur

16. GPS araç takip sistemi (ileri faz)
GPS araç takip sistemi çekirdek fazın bir parçası değildir, ama sistem buna açık tasarlanmalıdır.
Bu modül iki seviyede ele alınacaktır.
16.1 Faz 1 – İç operasyon takibi
Şirket içi kullanıcılar için:
* araç nerede
* hangi müşteri işi için çıktı
* şantiyeye yaklaştı mı
* sahada mı / dönüşte mi
Bu görünüm ana yönetici ve operasyon tarafında kullanılabilir.
16.2 Faz 2 – Müşteri taraflı araç takibi
İleri aşamada müşteri deneyimi tarafında:
* aracın çıkış yapıp yapmadığı
* konumu
* şantiyeye yaklaşma durumu
* belli mesafede bildirim
* canlı takip
gibi özellikler düşünülebilir.
Bu yapı yemek siparişi mantığındaki canlı takip deneyimine benzer.
16.3 Faz 3 – Gelişmiş teslimat deneyimi
İleride:
* tahmini varış süresi
* canlı harita
* araç hareket geçmişi
gibi gelişmiş özellikler eklenebilir.
Yapısal karar
Araç takip sistemi uzun vadede ayrı modül olarak düşünülmelidir.İlk aşamada Dashboard’da özet görünüm bulunabilir.

17. Müşteri tarafı erişim mantığı
Müşteri çekirdek sistem kullanıcısı değildir.
Yani müşteri:
* talep oluşturmaz
* sistemi yönetmez
* operasyon işlemi yapmaz
Talebi sisteme operasyoncu girer.
Ancak ileri aşamada müşteri için şu tip sınırlı deneyimler düşünülebilir:
* siparişe özel takip linki
* sadece kendi siparişini izleyebildiği mobil görünüm
* araç yaklaşma bildirimi
Bu, tam kullanıcı rolü olmak zorunda değildir; teslimat takibi amaçlı deneyim olabilir.

18. Çoklu platform yapısı
Sistem üç farklı erişim kanalıyla kullanılabilecek şekilde planlanmalıdır:
* Web
* Mobil
* Masaüstü program
Ana prensip
İş mantığı tek yerde olmalıdır.
Yani:
* talep onay kuralları
* araç atama mantığı
* yetki kontrolü
* maliyet hesapları
* durum geçişleri
bunlar webde ayrı, mobilde ayrı, masaüstünde ayrı yazılmamalıdır.
Tek merkezli iş mantığı ve servis yapısı kurulmalıdır.
Sonuç
Böylece:
* merkezi güncelleme yapılabilir
* tüm platformlar aynı kuralla çalışır
* veri tutarlılığı korunur
Not:Arayüzler farklı olabilir, ama iş kuralları aynı olmalıdır.

19. Ekran ve modül ilişkisi
Dashboard
Ana izleme ekranı
Program
Talep yaşam döngüsünün merkezi
Filo
Araç ve bakım yönetimi
Personel
Araçla ilişkili operasyonel personel yapısı
Müşteriler
Cari ve şantiye verisi
Maliyet
Günlük gider ve kârlılık analizi
Ayarlar
Sadece Ana Yönetici için kullanıcı ve yetki yönetimi

20. Nihai sistem akışı
Sistemin tam akışı şu şekilde okunmalıdır:
Müşteri→ Şantiye→ Talep oluşturma→ Onay Bekleyen→ Yönetici incelemesi→ Randevu saati verilmesi→ Onaylanan→ Müşteriye onay SMS’i→ Araç atama→ Operasyon→ Gün sonu gerçek teslim miktarı girişi→ Teslim Edilen→ Teslim SMS’i→ Gün sonu raporu→ Maliyet analizine dahil olma
Bu ana akış sabit referans kabul edilmelidir.

21. Kritik değişmez kararlar
Bu projede artık sabit kabul edilen kararlar şunlardır:
* Talebi müşteri değil, operasyoncu girer
* Talep onayı yönetici tarafından verilir
* Yönetici randevu saatini değiştirebilir
* Araç atama manuel yapılır
* Gün sonu kapanışı manuel yapılır
* Teslim edilen gerçek miktar ayrı tutulur
* Şantiye çoklu desteklidir
* Maliyet günlük ve kalem bazlı girilir
* Dashboard izleme ekranıdır
* Takvim Dashboard içindedir, Program’ın yerine geçmez
* Kullanıcı yönetimi ve yetki verme sadece Ana Yöneticiye aittir
* Muhasebe kullanıcısı ayrı roldür
* GPS ileri fazdır
* Sistem web, mobil ve masaüstünde çalışabilecek şekilde kurgulanır
* İş mantığı merkezi olur
* Dış sistemler veri kaynağı olarak kullanılır

22. Nihai ürün tanımı
Bu proje şu şekilde tanımlanabilir:
Hazır beton üretim, planlama, sevk, teslim, maliyet ve izleme süreçlerini uçtan uca yöneten; rol ve yetki bazlı kullanıcı kontrolüne sahip; müşteri ve şantiye bazlı talepleri planlayan; araç ve personel atamalarını yöneten; gün sonu teslim verilerini kaydeden; maliyet ve kârlılık analizleri yapan; Netsis ve Olympos gibi dış sistemlerle entegre çalışabilen; ilerleyen aşamalarda canlı araç takibi ve müşteri taraflı teslimat deneyimini destekleyebilecek çok platformlu bir operasyon yönetim sistemidir.
