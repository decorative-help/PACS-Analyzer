DECLARE anomalies_cursor CURSOR -- ОБЬЯВЛЯЕМ КУРСОР  
FOR 
  SELECT 
	[date]
	,user_source_id
	,user_target_id
	,duration
	,times
	,[zone]
	,[floor] 
  FROM [graphbydate]
  ORDER BY [date]

OPEN anomalies_cursor -- ОТКРЫВАЕМ КУРСОР  

-- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ  
DECLARE @parent_COUNTER    INT, 
        @parent_loop       INT, 
        @parent_start_time DATETIME, 
        @parent_end_time   DATETIME, 
        @parent_user_id    INT, 
        @parent_floor      INT, 
        @parent_zone       VARCHAR(50), 
        @parent_duration   INT 

SET @parent_COUNTER = 0 

-- ВЫБОРКА ПЕРВОЙ  СТРОКИ  
FETCH next FROM parent_table_cursor INTO @parent_start_time, @parent_end_time, 
@parent_user_id, @parent_floor, @parent_zone, @parent_duration 

-- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ  
SET @parent_loop = @@FETCH_STATUS 

WHILE @parent_loop = 0 
  BEGIN 

  /*
  Попросить у этой данные тестовые нормальные, тип для пятерых человек.
  Иначе оооочень муторно всё это обрабатывать и искать ничего


  Нам нужно искать аномалии во встречах пользователя 1 и 2, а у нас есть записи максимум трёх дней их встреч
  это крайне мало




SELECT
	*
FROM [graphbydate]
WHERE [duration] > AVG([duration])