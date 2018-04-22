TRUNCATE TABLE [AnomaliesTEMP] 

DECLARE unique_floors_cursor CURSOR 
-- ОБЬЯВЛЯЕМ КУРСОР Этажей     
FOR 
  SELECT DISTINCT [floor] 
  FROM   [intervals] 

OPEN unique_floors_cursor -- ОТКРЫВАЕМ КУРСОР       
-- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ       
DECLARE @floor_COUNTER INT, 
        @floor_loop    INT, 
        @floor_id      INT 

SET @floor_COUNTER = 0 

-- ВЫБОРКА ПЕРВОЙ  СТРОКИ       
FETCH next FROM unique_floors_cursor INTO @floor_id 

-- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ       
SET @floor_loop = @@FETCH_STATUS 

WHILE @floor_loop = 0 
  BEGIN 
      DECLARE unique_zones_cursor CURSOR 
      -- ОБЬЯВЛЯЕМ КУРСОР Зон     
      FOR 
        SELECT DISTINCT [zone] 
        FROM   [intervals] 

      OPEN unique_zones_cursor -- ОТКРЫВАЕМ КУРСОР       
      -- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ       
      DECLARE @zone_COUNTER INT, 
              @zone_loop    INT, 
              @zone_id      INT 

      SET @zone_COUNTER = 0 

      -- ВЫБОРКА ПЕРВОЙ  СТРОКИ       
      FETCH next FROM unique_zones_cursor INTO @zone_id 

      -- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ       
      SET @zone_loop = @@FETCH_STATUS 

      WHILE @zone_loop = 0 
        BEGIN 
            DECLARE unique_users_cursor CURSOR 
            -- ОБЬЯВЛЯЕМ КУРСОР Работников  
            FOR 
              SELECT DISTINCT [user_id] 
              FROM   [intervals] 

            OPEN unique_users_cursor -- ОТКРЫВАЕМ КУРСОР       
            -- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ       
            DECLARE @parent_COUNTER INT, 
                    @parent_loop    INT, 
                    @parent_user_id INT 

            SET @parent_COUNTER = 0 

            -- ВЫБОРКА ПЕРВОЙ  СТРОКИ       
            FETCH next FROM unique_users_cursor INTO @parent_user_id 

            -- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ       
            SET @parent_loop = @@FETCH_STATUS 

            WHILE @parent_loop = 0 
              BEGIN 
                  DECLARE target_users_cursor CURSOR 
                  -- ОБЬЯВЛЯЕМ КУРСОР       
                  FOR 
                    SELECT DISTINCT [user_id] 
                    FROM   [intervals] 

                  OPEN target_users_cursor 

                  -- ОТКРЫВАЕМ КУРСОР       
                  -- КУРСОР СОЗДАН, ОБЬЯВЛЯЕМ ПЕРЕМЕННЫЕ И ОБХОДИМ НАБОР СТРОК В ЦИКЛЕ       
                  DECLARE @child_COUNTER INT, 
                          @child_loop    INT, 
                          @child_user_id INT 

                  SET @child_COUNTER = 0 

                  -- ВЫБОРКА ПЕРВОЙ  СТРОКИ       
                  FETCH next FROM target_users_cursor INTO @child_user_id 

                  -- ЦИКЛ С ЛОГИКОЙ И ВЫБОРКОЙ ВСЕХ ПОСЛЕДУЮЩИХ СТРОК ПОСЛЕ ПЕРВОЙ       
                  SET @child_loop = @@FETCH_STATUS 

                  WHILE @child_loop = 0 
                    BEGIN 
                        DECLARE @avg_duration INT 

                        -- SELECT average duration for a pair of users    
                        SELECT @avg_duration = Avg([duration]) 
                        FROM   graphbydate 
                        WHERE  [user_source_id] <> [user_target_id] 
                               AND ( ( [user_source_id] = @parent_user_id 
                                       AND [user_target_id] = @child_user_id ) 
                                      OR ( [user_source_id] = @child_user_id 
                                           AND [user_target_id] = 
                                               @parent_user_id 
                                         ) ) 

                        IF EXISTS (SELECT * 
                                   FROM   [graphbydate] 
                                   WHERE  duration > @avg_duration 
                                          AND [user_source_id] <> 
                                              [user_target_id] 
                                          AND ( ( [user_source_id] = 
                                                  @parent_user_id 
                                                  AND [user_target_id] = 
                                                      @child_user_id 
                                                ) 
                                                 OR ( [user_source_id] = 
                                                      @child_user_id 
                                                      AND [user_target_id] = 
                                                          @parent_user_id 
                                                    ) 
                                              )) 
                          BEGIN -- if there is such pair of people    
                              IF (SELECT Count(*) 
                                  FROM   [anomaliestemp] 
                                  WHERE  duration > @avg_duration 
                                         AND [user_source_id] <> 
                                             [user_target_id] 
                                         AND ( ( [user_source_id] = 
                                                 @parent_user_id 
                                                 AND [user_target_id] = 
                                                     @child_user_id 
                                               ) 
                                                OR ( [user_source_id] = 
                                                     @child_user_id 
                                                     AND [user_target_id] = 
                                                         @parent_user_id 
                                                   ) 
                                             )) =  0 
                                --if anomaly is in [AnomaliesTEMP] 
                                BEGIN 
                                    -- if anomaly is NOT in [AnomaliesTEMP] then write it down 
                                    BEGIN try-- try to INSERT 
                                        INSERT INTO [anomaliestemp] 
                                                    ([date], 
                                                     user_source_id, 
                                                     user_target_id, 
                                                     duration, 
                                                     times, 
                                                     [zone], 
                                                     [floor]) 
                                        SELECT [date], 
                                               user_source_id, 
                                               user_target_id, 
                                               duration, 
                                               times, 
                                               [zone], 
                                               [floor] 
                                        FROM   graphbydate 
                                        WHERE  duration > @avg_duration 
                                               AND [user_source_id] <> 
                                                   [user_target_id] 
                                               AND ( ( [user_source_id] = 
                                                       @parent_user_id 
                                                       AND [user_target_id] = 
                                                           @child_user_id 
                                                     ) 
                                                      OR ( [user_source_id] = 
                                                           @child_user_id 
                                                           AND [user_target_id] 
                                                               = 
                                                               @parent_user_id 
                                                         ) 
                                                   ) 
                                    END try-- try to INSERT 

                                    BEGIN catch 
                                        -- ...UPDATE or nothing   
                                        PRINT 
                                    'Cannot INSERT INTO [AnomaliesTEMP]' 
                                    END catch 
                                END 
                          -- if anomaly is NOT in [AnomaliesTEMP] then write it down 
                          END -- if there is such pair of people     

                        SET @child_COUNTER = @child_COUNTER + 1 

                        -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ       
                        FETCH next FROM target_users_cursor INTO @child_user_id 

                        SET @child_loop = @@FETCH_STATUS 
                    -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА       
                    END 

                  -- ЗАКРЫВАЕМ КУРСОР       
                  CLOSE target_users_cursor 

                  DEALLOCATE target_users_cursor 

                  SET @parent_COUNTER = @parent_COUNTER + 1 

                  -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ       
                  FETCH next FROM unique_users_cursor INTO @parent_user_id 

                  SET @parent_loop = @@FETCH_STATUS 
              -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА       
              END 

            -- ЗАКРЫВАЕМ КУРСОР       
            CLOSE unique_users_cursor 

            DEALLOCATE unique_users_cursor 

            SET @zone_COUNTER = @zone_COUNTER + 1 

            -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ       
            FETCH next FROM unique_zones_cursor INTO @zone_id 

            SET @zone_loop = @@FETCH_STATUS 
        -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА       
        END 

      -- ЗАКРЫВАЕМ КУРСОР       
      CLOSE unique_zones_cursor 

      DEALLOCATE unique_zones_cursor 

      SET @floor_COUNTER = @floor_COUNTER + 1 

      -- ВЫБОРКА СЛЕДУЮЩЕЙ СТРОКИ       
      FETCH next FROM unique_floors_cursor INTO @floor_id 

      SET @floor_loop = @@FETCH_STATUS 
  -- ЗАВЕРШЕНИЕ ЛОГИКИ ВНУТРИ ЦИКЛА       
  END 

-- ЗАКРЫВАЕМ КУРСОР       
CLOSE unique_floors_cursor 

DEALLOCATE unique_floors_cursor 